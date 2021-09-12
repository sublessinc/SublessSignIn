const subless_Headers = new Headers();
subless_Headers.set('Cache-Control', 'no-store');
var subless_urlParams = new URLSearchParams(window.location.search);
var subless_mgr = null;
const subless_baseUri = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '') + window.location.pathname;
const subless_Uri = "https://dev.subless.com";
const subless_silentRenewPage = '<html><body>' +
    '<script src="https://cdnjs.cloudflare.com/ajax/libs/oidc-client/1.11.5/oidc-client.js" type = "text/javascript" ></script>' +
    '<script src="https://dev.subless.com/dist/subless-silent-renew.js" type="text/javascript"></script>' +
    '</body></html>';

const subless_silentRenewBlob = new Blob([subless_silentRenewPage], { type: 'text/html' });

var subless_config = {
    redirect_uri: subless_baseUri,
    post_logout_redirect_uri: subless_baseUri,



    // these two will be done dynamically from the buttons clicked, but are
    // needed if you want to use the silent_renew
    response_type: "code",
    scope: "openid",

    // this will toggle if profile endpoint is used
    loadUserInfo: true,

    // silent renew will get a new access_token via an iframe 
    // just prior to the old access_token expiring (60 seconds prior)
    silent_redirect_uri: window.URL.createObjectURL(subless_silentRenewBlob),
    automaticSilentRenew: true,

    // will revoke (reference) access tokens at logout time
    revokeAccessTokenOnSignout: true,

    // this will allow all the OIDC protocol claims to be visible in the window. normally a client app 
    // wouldn't care about them or want them taking up space
    filterProtocolClaims: false
};

async function subless_GetConfig() {
    var resp = await fetch(subless_Uri + "/api/Authorization/settings");
    var json = await resp.json();
    subless_config.authority = json.cognitoUrl;
    subless_config.client_id = json.appClientId;
    subless_config.metadataSeed = {
        end_session_endpoint :
        json.issuerUrl
        + "/logout?response_type=code&client_id="
        + subless_config.client_id
        + "&logout_uri="
        + subless_baseUri
    };
}

async function subless_initUserManagement() {
    Oidc.Log.logger = window.console;
    Oidc.Log.level = Oidc.Log.INFO;
    await subless_ConfigLoaded;
    subless_mgr = new Oidc.UserManager(subless_config);

    subless_mgr.events.addUserLoaded(function (user) {
        subless_log("User loaded");
        subless_isLoggedIn = true;
    });
    subless_mgr.events.addUserUnloaded(function () {
        subless_log("User logged out locally");
        subless_isLoggedIn = false;
    });
    subless_mgr.events.addAccessTokenExpiring(function () {
        subless_log("Access token expiring...");
    });
    subless_mgr.events.addSilentRenewError(function (err) {
        subless_log("Silent renew error: " + err.message);
    });
    subless_mgr.events.addUserSignedOut(function () {
        subless_log("User signed out of OP");
        subless_isLoggedIn = false;
    });
}

function subless_startLogin(scope, response_type) {

    var use_popup = false;
    if (!use_popup) {
        subless_mgr.signinRedirect({ scope: scope, response_type: response_type });
    }
    else {
        subless_mgr.signinPopup({ scope: scope, response_type: response_type }).then(function () {
            subless_log("Logged In");
        });
    }
}

async function sublessLogin() {
    await subless_ConfigLoaded;
    await subless_initUserManagement;
    subless_startLogin();
}

async function subless_LoggedIn() {
    await subless_UserManagerInitialized;
    return subless_mgr.getUser().then(function (user) {
        if (user) {
            return true;
        }
        return false;
    });;
}

async function subless_loginCallback() {
    var code = subless_urlParams.get('code');
    await subless_ConfigLoaded;
    var loggedIn = await subless_LoggedIn();
    if (code != null && !loggedIn) {
        subless_handleCallback();
    }
}

function subless_handleCallback() {
    subless_mgr.signinRedirectCallback().then(function (user) {
        var hash = window.location.hash.substr(1);
        var result = hash.split('&').reduce(function (result, item) {
            var parts = item.split('=');
            result[parts[0]] = parts[1];
            return result;
        }, {});
    });
}

async function subless_hit() {
    await subless_UserManagerInitialized;
    subless_mgr.getUser().then(function (user) {
        if (user) {
            var body =
                fetch(subless_Uri + "/api/hit", {
                    method: "POST",
                    headers: {
                        "Authorization": "Bearer " + user.access_token,
                        "Content-Type": "application/json",
                    },
                    body: window.location.href
                });
        }
    });
}


async function sublessLogout() {
    await subless_UserManagerInitialized;
    subless_mgr.signoutRedirect();
}

async function subless_revoke() {
    await subless_UserManagerInitialized;
    subless_mgr.revokeAccessToken();
}



function subless_log(data) {
    Array.prototype.forEach.call(arguments, function (msg) {
        if (msg instanceof Error) {
            msg = "Error: " + msg.message;
        }
        else if (typeof msg !== 'string') {
            msg = JSON.stringify(msg, null, 2);
        }
    });
}

var subless_ConfigLoaded = subless_GetConfig();
var subless_UserManagerInitialized = subless_initUserManagement();

subless_loginCallback();
subless_hit();