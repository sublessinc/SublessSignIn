import { UserManager, Log, UserManagerSettings } from "oidc-client-ts";

const subless_Headers = new Headers();
subless_Headers.set('Cache-Control', 'no-store');
var subless_urlParams = new URLSearchParams(window.location.search);
const subless_Uri = process.env.SUBLESS_URL;
const client_baseUri = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '') + '/';
const client_currentPath = window.location.pathname;

interface SublessInterface {
    subless_GetConfig(): Promise<UserManagerSettings>;
    sublessLogin(): Promise<void>;
    subless_LoggedIn(): Promise<boolean>;
    subless_hit(): Promise<void>;
    sublessLogout(): Promise<void>;
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

    async sublessLogin() {
        await this.subless_Config;
        window.location.href = subless_Uri + "/bff/login?returnUrl=" + client_baseUri;
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

    async subless_hit() {
        var loggedIn = await this.subless_LoggedIn();
        if (loggedIn) {
            var body =
                fetch(subless_Uri + "/api/hit", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: window.location.href,
                    credentials: "include"
                });
        }
    }


    async sublessLogout() {
        window.location.href = subless_Uri + "/bff/logout?returnUrl=" + client_baseUri;
    }
}

var subless = new Subless();
export default subless;