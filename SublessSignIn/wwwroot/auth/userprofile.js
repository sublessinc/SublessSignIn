var myHeaders = new Headers();
myHeaders.set('Cache-Control', 'no-store');
var urlParams = new URLSearchParams(window.location.search);

var baseURI = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '');
var redirectURI = baseURI + "/index.html";



//Convert Payload from Base64-URL to JSON
const decodePayload = payload => {
    const cleanedPayload = payload.replace(/-/g, '+').replace(/_/g, '/');
    const decodedPayload = atob(cleanedPayload)
    const uriEncodedPayload = Array.from(decodedPayload).reduce((acc, char) => {
        const uriEncodedChar = ('00' + char.charCodeAt(0).toString(16)).slice(-2)
        return `${acc}%${uriEncodedChar}`
    }, '')
    const jsonPayload = decodeURIComponent(uriEncodedPayload);

    return JSON.parse(jsonPayload)
}

//Parse JWT Payload
const parseJWTPayload = token => {
    const [header, payload, signature] = token.split('.');
    const jsonPayload = decodePayload(payload)

    return jsonPayload
};

//Parse JWT Header
const parseJWTHeader = token => {
    const [header, payload, signature] = token.split('.');
    const jsonHeader = decodePayload(header)

    return jsonHeader
};

//Generate a Random String
const getRandomString = () => {
    const randomItems = new Uint32Array(28);
    crypto.getRandomValues(randomItems);
    const binaryStringItems = randomItems.map(dec => `0${dec.toString(16).substr(-2)}`)
    return binaryStringItems.reduce((acc, item) => `${acc}${item}`, '');
}

//Encrypt a String with SHA256
const encryptStringWithSHA256 = async str => {
    const PROTOCOL = 'SHA-256'
    const textEncoder = new TextEncoder();
    const encodedData = textEncoder.encode(str);
    return crypto.subtle.digest(PROTOCOL, encodedData);
}

//Convert Hash to Base64-URL
const hashToBase64url = arrayBuffer => {
    const items = new Uint8Array(arrayBuffer)
    const stringifiedArrayHash = items.reduce((acc, i) => `${acc}${String.fromCharCode(i)}`, '')
    const decodedHash = btoa(stringifiedArrayHash)

    const base64URL = decodedHash.replace(/\+/g, '-').replace(/\//g, '_').replace(/=+$/, '');
    return base64URL
}

// Main Function
async function main(settings) {
    var code = urlParams.get('code');

    //If code not present then request code else request tokens
    if (code == null) {

        // Create random "state"
        var state = getRandomString();
        sessionStorage.setItem("pkce_state", state);

        // Create PKCE code verifier
        var code_verifier = getRandomString();
        sessionStorage.setItem("code_verifier", code_verifier);

        // Create code challenge
        var arrayHash = await encryptStringWithSHA256(code_verifier);
        var code_challenge = hashToBase64url(arrayHash);
        sessionStorage.setItem("code_challenge", code_challenge)

        // Redirtect user-agent to /authorize endpoint
        location.href = settings.issuerUrl + "/oauth2/authorize?response_type=code&state=" + state + "&client_id=" + settings.appClientId + "&redirect_uri=" + redirectURI + "&scope=openid&code_challenge_method=S256&code_challenge=" + code_challenge;
    } else {

        // Verify state matches
        state = urlParams.get('state');
        if (sessionStorage.getItem("pkce_state") != state) {
            alert("Invalid state");
        } else {

            // Fetch OAuth2 tokens from Cognito
            code_verifier = sessionStorage.getItem('code_verifier');
            await fetch(settings.issuerUrl + "/oauth2/token?grant_type=authorization_code&client_id=" + settings.appClientId + "&code_verifier=" + code_verifier + "&redirect_uri=" + redirectURI + "&code=" + code, {
                method: 'post',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded'
                }
            })
            .then((response) => {
                return response.json();
            })
            .then((data) => {
                tokens = data; 
                sessionStorage.setItem("id_token", tokens.id_token);
                sessionStorage.setItem("access_token", tokens.access_token);
                fetch("/api/Authorization/redirect", {
                    headers: {
                        "Authorization": "Bearer " + sessionStorage.getItem('id_token')
                    }
                }).then(function (resp) {
                    resp.json().then(function (json) {
                        redirect = json;
                        switch (redirect.redirectionPath) {
                            case 1:
                                window.location.replace(baseURI + "/LoggedInButNotPaid.html");
                                break;
                            case 2:
                                window.location.replace(baseURI + "/PayingCustomer.html?sessionId="+redirect.sessionId);
                                break;
                            default:

                        }
                    })
                });
            });
        }
    }
}

fetch("/api/Authorization/settings")
    .then(function (resp) {
        var json = resp.json().then(json => {
            main(json);
        });
    });
