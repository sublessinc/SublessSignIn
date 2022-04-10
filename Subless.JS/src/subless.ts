
const sublessHeaders = new Headers();
sublessHeaders.set("Cache-Control", "no-store");
const sublessUri = process.env.SUBLESS_URL;
const clientBaseUri = location.protocol + "//" + window.location.hostname + (location.port ? ":" + location.port : "") + "/";

interface SublessInterface {
    subless_GetConfig(): Promise<SublessSettings>;
    sublessLogin(): Promise<void>;
    subless_LoggedIn(): Promise<boolean>;
    subless_hit(): Promise<void>;
    sublessLogout(): Promise<void>;
}

interface SublessSettings {
    redirectUri: string;
    postLogoutRedirectUri: string;
    authority: string;
    clientId: string;
}

const sublessConfig: SublessSettings = {
    redirectUri: clientBaseUri,
    postLogoutRedirectUri: clientBaseUri,
    authority: "",
    clientId: "",
};


/** A set of methods that can be used to integrate a partner site with Subless,
 * such that the partner can allow Subless users to distribute funds to the creators
 * the users visit on their site.
 */
export class Subless implements SublessInterface {
    sublessConfig: Promise<SublessSettings>;
    /** Attempt to automatically authenticate with subless and record a hit on
     * initialization.
     */
    constructor() {
        this.sublessConfig = this.subless_GetConfig();
        this.subless_hit();
    }

    /** Query Subless for the latest authorization details for the Subless server. */
    async subless_GetConfig(): Promise<SublessSettings> {
        const resp = await fetch(sublessUri + "/api/Authorization/settings");
        const json = await resp.json();
        sublessConfig.authority = json.cognitoUrl;
        sublessConfig.clientId = json.appClientId;
        return sublessConfig;
    }

    /** A method that can be used to redirect a user to a Subless login. I.e., can be attached
     * to a "sign in to subless" button on a partner site.
     */
    async sublessLogin() {
        await this.sublessConfig;
        window.location.href = sublessUri + "/bff/login?returnUrl=" + clientBaseUri;
    }

    /** Check whether a user who had loaded this page is logged into a Subless account. */
    async subless_LoggedIn(): Promise<boolean> {
        return await
        fetch(sublessUri + "/api/user/loggedin", {
            method: "Get",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: "include",
        }).then((response) => response.json());
    }


    /** If a user is logged in, report the user's view of this creator/site to the subless server. */
    async subless_hit() {
        const loggedIn = await this.subless_LoggedIn();
        if (loggedIn) {
            // The below rule is disabled to force evaluation.
            const body = // eslint-disable-line no-unused-vars
                fetch(sublessUri + "/api/hit", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: window.location.href,
                    credentials: "include",
                });
        }
    }

    /** A method that can be used to log a user out from Subless */
    async sublessLogout() {
        window.location.href = sublessUri + "/bff/logout?returnUrl=" + clientBaseUri;
    }
}

const subless = new Subless();
export default subless;
