const sublessHeaders = new Headers();
sublessHeaders.set("Cache-Control", "no-store");
const sublessUri = process.env.SUBLESS_URL;
const clientBaseUri = location.protocol + "//" + window.location.hostname + (location.port ? ":" + location.port : "") + "/";

interface SublessInterface {
    // camelcase is disabled for these so we don't conflict with customer namespaces
    subless_GetConfig(): Promise<SublessSettings>; // eslint-disable-line camelcase
    sublessLogin(): Promise<void>;
    subless_LoggedIn(): Promise<boolean>; // eslint-disable-line camelcase
    subless_hit(): Promise<void>; // eslint-disable-line camelcase
    sublessLogout(): Promise<void>;
    sublessShowLogo(): Promise<void>;
    sublessShowBanner(): Promise<void>;
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
    async subless_GetConfig(): Promise<SublessSettings> { // eslint-disable-line camelcase
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
    async subless_LoggedIn(): Promise<boolean> { // eslint-disable-line camelcase
        return await fetch(sublessUri + "/api/user/loggedin", {
            method: "Get",
            headers: {
                "Content-Type": "application/json",
            },
            credentials: "include",
        }).then((response) => response.json());
    }

    /** If a user is logged in, report the user's view of this creator/site to the subless server. */
    async subless_hit() { // eslint-disable-line camelcase
        const loggedIn = await this.subless_LoggedIn();
        if (loggedIn) {
            // The below rule is disabled to force evaluation.
            const body = // eslint-disable-line no-unused-vars
                fetch(sublessUri + "/api/hit", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: window.location.origin + window.location.pathname,
                    credentials: "include",
                });
            const result = await body.then((response) => response.json());
            if (result === true) {
                this.sublessShowLogo();
            }
        }
    }

    /** Shows logo at bottom right corner */
    async sublessShowLogo() {
        const img = document.createElement("img");
        img.style.position = "absolute";
        img.style.bottom = "0";
        img.style.right = "0";
        img.style.width = "5%";
        img.src = sublessUri + "/dist/assets/SublessIcon.svg";
        img.id = "sublessHitIndicator";
        document.body.appendChild(img);
        this.fadeInAndOut(img);
    }

    /** Inserts banner ad into the page */
    async sublessShowBanner(): Promise<void> {
        const bannerDiv = document.getElementById("sublessMessage");
        const link = document.createElement("a");
        const img = document.createElement("img");
        const urls = this.getBanner();
        img.src = urls[0];
        img.id = "sublessMessageImage";
        img.style.maxHeight = "90px";
        link.href = urls[1];
        link.id = "sublessMessageLink";
        link.appendChild(img);
        bannerDiv.appendChild(link);
    }

    /** Gets a random banner ad and corresponding link
     * @return {[string, string]}tuple of image and target URI
    */
    private getBanner(): [string, string] {
        const banner = Math.floor(Math.random() * 4) + 1;
        const img = `${sublessUri}/dist/assets/banner${banner}.png`;
        if (banner == 3) {
            return [img, `https://www.subless.com/hf-creator-instructions?utm_campaign=banner${banner}`];
        }
        return [img, `https://www.subless.com/patron?utm_campaign=banner${banner}`];
    }

    /** Fades in an element
     * @param {HTMLElement} el element to fade
    */
    fadeInAndOut(el: HTMLElement) {
        el.style.opacity = "0";
        let fadeout = false;
        const tick = function () {
            if (!fadeout) {
                el.style.opacity = +el.style.opacity + 0.02 + "";
            } else {
                el.style.opacity = +el.style.opacity - 0.02 + "";
            }
            if (fadeout && +el.style.opacity == 0) {
                document.body.removeChild(el);
                return;
            }
            if (!fadeout && +el.style.opacity >= 1) {
                fadeout = true;
            }
            if (fadeout || +el.style.opacity < 1) {
                (window.requestAnimationFrame && requestAnimationFrame(tick)) || setTimeout(tick, 16);
            }
        };
        tick();
    }

    /** A method that can be used to log a user out from Subless */
    async sublessLogout() {
        window.location.href = sublessUri + "/bff/logout?returnUrl=" + clientBaseUri;
    }
}

const subless = new Subless();
export default subless;
