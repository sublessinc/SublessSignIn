import { UserManager, Log, UserManagerSettings } from "oidc-client-ts";

const subless_Headers = new Headers();
subless_Headers.set('Cache-Control', 'no-store');
var subless_urlParams = new URLSearchParams(window.location.search);
//var subless_mgr: UserManager | null = null;
const subless_Uri = process.env.SUBLESS_URL;
const client_baseUri = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '') + '/';
const client_currentPath = window.location.pathname;
// const subless_silentRenewPage =
//     '<html>' +
//     '<body>' +
//     '    <script type="module">' +
//     '        import Subless from "' + subless_Uri + '";' +
//     '        window.subless = Subless;' +
//     '        window.subless.subless_SilentCallback();' +
//     '    </script>' +
//     '</body>' +
//     '</html>';
//const subless_silentRenewBlob = new Blob([subless_silentRenewPage], { type: 'text/html' });

interface SublessInterface {
    subless_GetConfig(): Promise<UserManagerSettings>;
    // subless_initUserManagement(): Promise<UserManager>;
    // subless_startLogin(): Promise<void>;
    sublessLogin(): Promise<void>;
    subless_LoggedIn(): Promise<boolean>;
    // subless_loginCallback(): Promise<void>;
    // subless_handleCallback(): Promise<void>;
    subless_hit(): Promise<void>;
    sublessLogout(): Promise<void>;
    // subless_revoke(): Promise<void>;
    // subless_log(data: string): void;
}

var subless_config: UserManagerSettings = {
    redirect_uri: client_baseUri,
    post_logout_redirect_uri: client_baseUri,


    // these two will be done dynamically from the buttons clicked, but are
    // needed if you want to use the silent_renew
    response_type: "code",
    scope: "openid",

    // this will toggle if profile endpoint is used
    loadUserInfo: true,

    // silent renew will get a new access_token via an iframe 
    // just prior to the old access_token expiring (60 seconds prior)
    //silent_redirect_uri: window.URL.createObjectURL(subless_silentRenewBlob),
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

var callbackRunning: Promise<void> = null;


export class Subless implements SublessInterface {
    subless_Config: Promise<UserManagerSettings>;
    subless_UserManager: Promise<UserManager>;
    constructor() {
        this.subless_Config = this.subless_GetConfig();
        // this.subless_UserManager = this.subless_initUserManagement();

        // callbackRunning = this.subless_loginCallback();
        this.subless_hit();
    }

    async subless_GetConfig(): Promise<UserManagerSettings> {
        var resp = await fetch(subless_Uri + "/api/Authorization/settings");
        var json = await resp.json();
        subless_config.authority = json.cognitoUrl;
        subless_config.client_id = json.appClientId;
        subless_config.metadataSeed = {
            end_session_endpoint:
                json.issuerUrl
                + "/logout?response_type=code&client_id="
                + subless_config.client_id
                + "&logout_uri="
                + client_baseUri
        };
        return subless_config;
    }


    // async subless_initUserManagement(): Promise<UserManager> {
    //     Log.logger = window.console;
    //     Log.level = Log.INFO;
    //     var subless_config = await this.subless_Config;
    //     subless_mgr = new UserManager(subless_config);

    //     subless_mgr.events.addUserLoaded(function (user) {
    //     });
    //     subless_mgr.events.addUserUnloaded(function () {
    //     });
    //     subless_mgr.events.addAccessTokenExpiring(function () {
    //     });
    //     subless_mgr.events.addSilentRenewError(function (err) {
    //     });
    //     subless_mgr.events.addUserSignedOut(function () {
    //     });
    //     return subless_mgr;
    // }

    // async subless_startLogin(): Promise<void> {
    //     if (subless_mgr == null) {
    //         subless_mgr = await this.subless_initUserManagement();
    //     }
    //     this.subless_SetRedirectPath();
    //     var use_popup = false;
    //     if (!use_popup) {
    //         subless_mgr.signinRedirect();
    //     }
    //     else {
    //         subless_mgr.signinPopup().then(function () {
    //         });
    //     }
    // }

    // subless_SetRedirectPath(): void {
    //     sessionStorage.setItem("pre-login-path", client_currentPath.substring(1));
    // }

    // subless_PostLoginRedirect(): void {
    //     var path = sessionStorage.getItem("pre-login-path");
    //     sessionStorage.setItem("pre-login-path", "");
    //     if (path) {
    //         window.location.href = client_baseUri + path;
    //     }
    // }

    async sublessLogin() {
        await this.subless_Config;
        //await this.subless_initUserManagement;
        window.location.href = subless_Uri + "/bff/login?returnUrl=" + client_baseUri;
        //this.subless_startLogin();
    }

    async subless_LoggedIn(): Promise<boolean> {
        return await
            fetch(subless_Uri + "/api/user/loggedin", {
                method: "Get",
                headers: {
                    "Content-Type": "application/json",
                },
                credentials: "include"
            }).then(response => {
                return response.status == 200;
            });
    }

    // async subless_TokenPresent(): Promise<boolean> {
    //     subless_mgr = await this.subless_UserManager;
    //     return subless_mgr.getUser().then(function (user) {
    //         if (user) {
    //             return true;
    //         }
    //         return false;
    //     });
    // }

    // async subless_loginCallback() {
    //     var code = subless_urlParams.get('code');
    //     await this.subless_Config;
    //     var loggedIn = await this.subless_TokenPresent();
    //     if (code != null && !loggedIn) {
    //         await this.subless_handleCallback();
    //         this.subless_PostLoginRedirect();
    //     }
    // }

    // async subless_handleCallback() {
    //     subless_mgr = await this.subless_UserManager;
    //     await subless_mgr.signinRedirectCallback();
    // }

    async subless_hit() {
        //subless_mgr = await this.subless_UserManager;
        var loggedIn = await this.subless_LoggedIn();
        if (loggedIn) {
            // subless_mgr.getUser().then(function (user) {
            //     if (user) {
            var body =
                fetch(subless_Uri + "/api/hit", {
                    method: "POST",
                    headers: {
                        //"Authorization": "Bearer " + user.access_token,
                        "Content-Type": "application/json",
                    },
                    body: window.location.href,
                    credentials: "include"
                });
            //     }
            // });
        }
    }


    async sublessLogout() {
        // subless_mgr = await this.subless_UserManager;
        window.location.href = subless_Uri + "/bff/logout?returnUrl=" + client_baseUri;
    }

    // async subless_revoke() {
    //     subless_mgr = await this.subless_UserManager;
    //     subless_mgr.revokeAccessToken();
    // }



    // subless_log(data: string) {
    //     Log.logger.info(data);
    // }

    // subless_SilentCallback() {
    //     this.subless_GetConfig().then((config: UserManagerSettings) => {
    //         var subless_mgr = new UserManager(config);
    //         subless_mgr.signinSilentCallback();
    //     });
    // }

}

var subless = new Subless();
export default subless;