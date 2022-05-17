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
            if (result !== null) {
                this.sublessShowLogo(result);
            }
        }
    }

    /** Shows logo at bottom right corner 
     * @param {string} creatorAvatar a uri linking to the avatar
    */
    async sublessShowLogo(creatorAvatar: string) {
        const div = document.createElement("div");
        div.style.display = "flex";
        div.style.flexDirection = "column";
        div.style.alignItems = "center";
        div.style.opacity = "0";
        div.style.position = "absolute";
        div.style.bottom = "0";
        div.style.right = "0%";
        div.style.width = "6%";

        const sublessImg = document.createElement("img");
        sublessImg.style.maxWidth = "100%";
        sublessImg.src = sublessUri + "/assets/redist/SupportedDark.svg";

        const creatorImg = document.createElement("img");
        creatorImg.style.maxWidth = "100%";
        creatorImg.src = creatorAvatar;

        div.appendChild(creatorImg);
        div.appendChild(sublessImg);
        document.body.appendChild(div);
        this.waitForLoadBeforeFade(creatorImg, sublessImg, div, this.fadeInAndOut);
    }

    /**
     * Waits for image loading to finish
     * @param {HTMLImageElement} image1 wait for image
     * @param {HTMLImageElement} image2 wait for image
     * @param {HTMLElement} fadeElement the element to fade
     * @param {Function} callback to fire after loading is complete
     */
    waitForLoadBeforeFade(image1: HTMLImageElement, image2: HTMLImageElement, fadeElement: HTMLElement, callback: Function) {
        const wait = function () {
            if (!image1 || !image2 || !image1.complete || !image2.complete) {
                setTimeout(wait, 16);
            } else {
                callback(fadeElement);
            }
        };
        wait();
    }

    /** Fades in an element
     * @param {HTMLElement} el element to fade
     * @param {number} delay ticks to delay the fade start
     * @param {number} increment speed to run
    */
    fadeInAndOut(el: HTMLElement, delay: number = 0, increment: number = 0.02) {
        el.style.opacity = "0";
        let fadeout = false;
        const tick = function () {
            if (delay == 0) {
                if (!fadeout) {
                    el.style.opacity = +el.style.opacity + increment + "";
                } else {
                    el.style.opacity = +el.style.opacity - increment + "";
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
            } else {
                delay--;
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
