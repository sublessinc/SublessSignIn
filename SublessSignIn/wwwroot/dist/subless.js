var subless_Headers = new Headers();
subless_Headers.set('Cache-Control', 'no-store');
var subless_urlParams = new URLSearchParams(window.location.search);

var subless_baseUri = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '') + window.location.pathname;
var subless_Uri = "https://pay.subless.com";
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
    silent_redirect_uri: window.location.origin + "/silent.html",
    automaticSilentRenew: true,

    // will revoke (reference) access tokens at logout time
    revokeAccessTokenOnSignout: true,

    // this will allow all the OIDC protocol claims to be visible in the window. normally a client app 
    // wouldn't care about them or want them taking up space
    filterProtocolClaims: false
};


function subless_populateConfig(followOnFunction) {
    fetch(subless_Uri + "/api/Authorization/settings")
        .then(function (resp) {
            var json = resp.json().then(json => {
                subless_config.authority = json.cognitoUrl;
                subless_config.client_id = json.appClientId;
                subless_init();
                followOnFunction();
            });
        });
}

function sublessLogin() {
    subless_populateConfig(subless_startLogin);
}

function subless_loginCallback() {
    var code = subless_urlParams.get('code');
    subless_populateConfig(function () {
        subless_mgr.getUser().then(function (user) {
            if (code != null && user == null) {
                subless_handleCallback();
            }
        });
    });
}


function subless_hit() {
    subless_populateConfig(function () {
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
    });
}












var subless_mgr = null;



function subless_init() {
    Oidc.Log.logger = window.console;
    Oidc.Log.level = Oidc.Log.INFO;

    subless_mgr = new Oidc.UserManager(subless_config);

    subless_mgr.events.addUserLoaded(function (user) {
        subless_log("User loaded");
    });
    subless_mgr.events.addUserUnloaded(function () {
        subless_log("User logged out locally");
    });
    subless_mgr.events.addAccessTokenExpiring(function () {
        subless_log("Access token expiring...");
    });
    subless_mgr.events.addSilentRenewError(function (err) {
        subless_log("Silent renew error: " + err.message);
    });
    subless_mgr.events.addUserSignedOut(function () {
        subless_log("User signed out of OP");
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

function subless_logout() {
    subless_mgr.signoutRedirect();
}

function subless_revoke() {
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

function subless_display(selector, data) {
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

function subless_isLoggedIn() {
    return subless_mgr.getUser().then(function (user) {
        if (user) {
            return true;
        }
        return false;
    });
}



subless_loginCallback();
subless_hit();