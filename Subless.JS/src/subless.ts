
const subless_Headers = new Headers();
subless_Headers.set('Cache-Control', 'no-store');
var subless_urlParams = new URLSearchParams(window.location.search);
const subless_Uri = process.env.SUBLESS_URL;
const client_baseUri = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '') + '/';
const client_currentPath = window.location.pathname;

interface SublessInterface {
    subless_GetConfig(): Promise<SublessSettings>;
    sublessLogin(): Promise<void>;
    subless_LoggedIn(): Promise<boolean>;
    subless_hit(): Promise<void>;
    sublessLogout(): Promise<void>;
}

interface SublessSettings {
    redirect_uri: string;
    post_logout_redirect_uri: string;
    authority: string;
    client_id: string;
}

var subless_config: SublessSettings = {
    redirect_uri: client_baseUri,
    post_logout_redirect_uri: client_baseUri,
    authority: "",
    client_id: ""

};


export class Subless implements SublessInterface {
    subless_Config: Promise<SublessSettings>;
    constructor() {
        this.subless_Config = this.subless_GetConfig();
        this.subless_hit();
    }

    async subless_GetConfig(): Promise<SublessSettings> {
        var resp = await fetch(subless_Uri + "/api/Authorization/settings");
        var json = await resp.json();
        subless_config.authority = json.cognitoUrl;
        subless_config.client_id = json.appClientId;
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
            }).then(response => response.json());
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
                    body: window.location.origin + window.location.pathname,
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