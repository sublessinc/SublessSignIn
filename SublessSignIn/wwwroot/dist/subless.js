import { UserManager, Log } from "oidc-client-ts";
const subless_Headers = new Headers();
subless_Headers.set('Cache-Control', 'no-store');
var subless_urlParams = new URLSearchParams(window.location.search);
var subless_mgr = null;
const subless_baseUri = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '') + window.location.pathname;
const subless_silentRenewPage = '<html><body>' +
    '<script src="https://cdnjs.cloudflare.com/ajax/libs/oidc-client/1.11.5/oidc-client.js" type = "text/javascript" ></script>' +
    '<script src="https://stage.subless.com/dist/subless.js" type="text/javascript"></script>' +
    '<script type="text/javascript">' +
    'subless_SilentCallback();' +
    '</script>' +
    '</body></html>';
const subless_silentRenewBlob = new Blob([subless_silentRenewPage], { type: 'text/html' });
const subless_Uri = "https://stage.subless.com";
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
    filterProtocolClaims: false,
    authority: "",
    client_id: "",
    metadataSeed: {}
};
export default class Subless {
    async subless_GetConfig() {
        var resp = await fetch(subless_Uri + "/api/Authorization/settings");
        var json = await resp.json();
        subless_config.authority = json.cognitoUrl;
        subless_config.client_id = json.appClientId;
        subless_config.metadataSeed = {
            end_session_endpoint: json.issuerUrl
                + "/logout?response_type=code&client_id="
                + subless_config.client_id
                + "&logout_uri="
                + subless_baseUri
        };
        return subless_config;
    }
    async subless_initUserManagement() {
        Log.logger = window.console;
        Log.level = Log.INFO;
        var subless_config = await subless_Config;
        subless_mgr = new UserManager(subless_config);
        subless_mgr.events.addUserLoaded(function (user) {
            //subless_log("User loaded");
        });
        subless_mgr.events.addUserUnloaded(function () {
            //subless_log("User logged out locally");
        });
        subless_mgr.events.addAccessTokenExpiring(function () {
            //subless_log("Access token expiring...");
        });
        subless_mgr.events.addSilentRenewError(function (err) {
            //subless_log("Silent renew error: " + err.message);
        });
        subless_mgr.events.addUserSignedOut(function () {
            //subless_log("User signed out of OP");
        });
        return subless_mgr;
    }
    async subless_startLogin() {
        if (subless_mgr == null) {
            subless_mgr = await this.subless_initUserManagement();
        }
        var use_popup = false;
        if (!use_popup) {
            subless_mgr.signinRedirect();
        }
        else {
            subless_mgr.signinPopup().then(function () {
                //subless_log("Logged In");
            });
        }
    }
    async sublessLogin() {
        await subless_Config;
        await this.subless_initUserManagement;
        this.subless_startLogin();
    }
    async subless_LoggedIn() {
        subless_mgr = await subless_UserManager;
        return subless_mgr.getUser().then(function (user) {
            if (user) {
                return true;
            }
            return false;
        });
        ;
    }
    async subless_loginCallback() {
        var code = subless_urlParams.get('code');
        await subless_Config;
        var loggedIn = await this.subless_LoggedIn();
        if (code != null && !loggedIn) {
            this.subless_handleCallback();
        }
    }
    async subless_handleCallback() {
        subless_mgr = await subless_UserManager;
        subless_mgr.signinRedirectCallback();
        // .then(function (user) {
        //     var hash = window.location.hash.substr(1);
        //     var result = hash.split('&').reduce(function (result, item) {
        //         var parts = item.split('=');
        //         result[parts[0]] = parts[1];
        //         return result;
        //     }, {});
        // });
    }
    async subless_hit() {
        subless_mgr = await subless_UserManager;
        subless_mgr.getUser().then(function (user) {
            if (user) {
                var body = fetch(subless_Uri + "/api/hit", {
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
    async sublessLogout() {
        subless_mgr = await subless_UserManager;
        subless_mgr.signoutRedirect();
    }
    async subless_revoke() {
        subless_mgr = await subless_UserManager;
        subless_mgr.revokeAccessToken();
    }
    subless_log(data) {
        Log.logger.info(data);
    }
    subless_SilentCallback() {
        this.subless_GetConfig().then((config) => {
            var subless_mgr = new UserManager(config);
            subless_mgr.signinSilentCallback();
        });
    }
}
var subless = new Subless();
var subless_Config = subless.subless_GetConfig();
var subless_UserManager = subless.subless_initUserManagement();
subless.subless_loginCallback();
subless.subless_hit();
//# sourceMappingURL=subless.js.map