var myHeaders = new Headers();
myHeaders.set('Cache-Control', 'no-store');
var urlParams = new URLSearchParams(window.location.search);

var baseURI = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '') + window.location.pathname;
var sublessURI = "https://pay.subless.com";

var config = {
    redirect_uri: baseURI,
    post_logout_redirect_uri: baseURI,

    // these two will be done dynamically from the buttons clicked, but are
    // needed if you want to use the silent_renew
    response_type: "code",
    scope: "openid",

    // this will toggle if profile endpoint is used
    loadUserInfo: true,

    // silent renew will get a new access_token via an iframe 
    // just prior to the old access_token expiring (60 seconds prior)
    silent_redirect_uri: window.location.origin + "/silent.html",
    automaticSilentRenew: true,

    // will revoke (reference) access tokens at logout time
    revokeAccessTokenOnSignout: true,

    // this will allow all the OIDC protocol claims to be visible in the window. normally a client app 
    // wouldn't care about them or want them taking up space
    filterProtocolClaims: false
};


function populateConfig(followOnFunction) {
    fetch(sublessURI + "/api/Authorization/settings")
        .then(function (resp) {
            var json = resp.json().then(json => {
                config.authority = json.cognitoUrl;
                config.client_id = json.appClientId;
                init();
                followOnFunction();
            });
        });
}

function sublessLogin() {
    populateConfig(login);
}

function sublessLoginCallback() {
    var code = urlParams.get('code');
    populateConfig(function () {
        mgr.getUser().then(function (user) {
            if (code != null && user == null) {
                handleCallback();
            }
        });
    });
}


function hitSubless() {
    populateConfig(function () {
        mgr.getUser().then(function (user) {
            if (user) {
                var body =
                    fetch(sublessURI + "/api/hit", {
                        method: "POST",
                        headers: {
                            "Authorization": "Bearer " + user.access_token,
                            "Content-Type": "application/json",
                        },
                        body: window.location.href
                    });

            }
        });
    });
}












var mgr = null;



function init() {
    Oidc.Log.logger = window.console;
    Oidc.Log.level = Oidc.Log.INFO;

    mgr = new Oidc.UserManager(config);

    mgr.events.addUserLoaded(function (user) {
        log("User loaded");
    });
    mgr.events.addUserUnloaded(function () {
        log("User logged out locally");
    });
    mgr.events.addAccessTokenExpiring(function () {
        log("Access token expiring...");
    });
    mgr.events.addSilentRenewError(function (err) {
        log("Silent renew error: " + err.message);
    });
    mgr.events.addUserSignedOut(function () {
        log("User signed out of OP");
    });
}

function sublessLogin() {
    sessionStorage.removeItem("id_token");
    sessionStorage.removeItem("access_token");
    fetch(sublessURI + "/api/Authorization/settings")
        .then(function (resp) {
            handleUnauthorized(resp);
            var json = resp.json().then(json => {
                executeSublessLogin(json);
            });
        });
}

function logout() {
    mgr.signoutRedirect();
}

        fetch(sublessURI + "/api/Authorization/settings")
            .then(function (resp) {
                handleUnauthorized(resp);
                var json = resp.json().then(json => {

function log(data) {
    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
    });
}

function display(selector, data) {
    if (data && typeof data === 'string') {
        try {
            data = JSON.parse(data);
        }
        catch (e) { }
    }
    if (data && typeof data !== 'string') {
        data = JSON.stringify(data, null, 2);
    }
    document.querySelector(selector).textContent = data;
}

function handleCallback() {
    mgr.signinRedirectCallback().then(function (user) {
        var hash = window.location.hash.substr(1);
        var result = hash.split('&').reduce(function (result, item) {
            var parts = item.split('=');
            result[parts[0]] = parts[1];
            return result;
        }, {});

function hitSubless() {
    var token = sessionStorage.getItem('id_token');
    if (token) {
        var body =
            fetch(sublessURI + "/api/hit", {
                method: "POST",
                headers: {
                    "Authorization": "Bearer " + token,
                    "Content-Type": "application/json",
                },
                body: window.location.href
            }).then(response => {
                handleUnauthorized(response);
            });
            
    }
}

function handleUnauthorized(response) {
    if (response.status === 401)

        sessionStorage.removeItem("id_token");
        sessionStorage.removeItem("access_token");
    }
}

sublessLoginCallback();
hitSubless();
