const sublessHeaders = new Headers();
sublessHeaders.set("Cache-Control", "no-store");
const sublessUri = process.env.SUBLESS_URL;
const clientBaseUri = location.protocol + "//" + window.location.hostname + (location.port ? ":" + location.port : "") + "/";

interface SublessInterface {
    // camelcase is disabled for these so we don't conflict with customer namespaces
    subless_GetConfig(hitStrategy: HitStrategy): Promise<SublessSettings>; // eslint-disable-line camelcase
    sublessLogin(): Promise<void>;
    subless_LoggedIn(): Promise<boolean>; // eslint-disable-line camelcase
    subless_hit(): Promise<void>; // eslint-disable-line camelcase
    sublessLogout(): Promise<void>;
    sublessShowLogo(): Promise<void>;
    sublessShowBanner(): Promise<void>;
}

export enum HitStrategy {
    uri,
    tag
}

interface SublessSettings {
    redirectUri: string;
    postLogoutRedirectUri: string;
    authority: string;
    clientId: string;
    hitStrategy: HitStrategy;
}

const sublessConfig: SublessSettings = {
    redirectUri: clientBaseUri,
    postLogoutRedirectUri: clientBaseUri,
    authority: "",
    clientId: "",
    hitStrategy: HitStrategy.uri,
};


/** A set of methods that can be used to integrate a partner site with Subless,
 * such that the partner can allow Subless users to distribute funds to the creators
 * the users visit on their site.
 */
export class Subless implements SublessInterface {
    sublessConfig: Promise<SublessSettings>;
    /** Attempt to automatically authenticate with subless and record a hit on
     * initialization.
     * @param {HitStrategy} hitStrategy strategy to use when detecting creator
     */
    constructor(hitStrategy: HitStrategy) {
        this.sublessConfig = this.subless_GetConfig(hitStrategy=hitStrategy);
        this.subless_hit();
    }

    /** Query Subless for the latest authorization details for the Subless server.
     * @param {HitStrategy} hitStrategy strategy to use when detecting creator
    */
    async subless_GetConfig(hitStrategy: HitStrategy): Promise<SublessSettings> {
        const resp = await fetch(sublessUri + "/api/Authorization/settings");
        const json = await resp.json();
        sublessConfig.authority = json.cognitoUrl;
        sublessConfig.clientId = json.appClientId;
        sublessConfig.hitStrategy = hitStrategy;
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
        const result = await this.sublessGetLoginState();
        if (result == 0) {
            return false;
        }
        if (result == 1) {
            return true;
        }
        if (result == 2) {
            await this.renewLogin();
            return true;
        }
        return false;
    }

    /** Starts a redirect chain to renew your session cookie */
    async renewLogin() {
        const path = window.location.href;
        window.location.href = sublessUri + "/renew?return_uri=" + path;
    }

    /** Gets the current state of the login */
    async sublessGetLoginState(): Promise<number> {
        return await fetch(sublessUri + "/api/user/loginStatus", {
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
            if ((await this.sublessConfig).hitStrategy === HitStrategy.tag) {
                await this.pushTagHit();
            } else {
                await this.pushUriHit();
            }
        }
    }

    /** Push hits based on tags */
    async pushTagHit() {
        const creators = await this.getSublessTags();
        for (const creator of creators) {
            // The below rule is disabled to force evaluation.
            const body = // eslint-disable-line no-unused-vars
                fetch(sublessUri + "/api/hit/tag", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({
                        uri: window.location.origin + window.location.pathname,
                        creator: creator,
                    }),
                    credentials: "include",
                });
            const result = await body.then((response) => response.json());
            if (result === true) {
                this.sublessShowLogo();
            }
        }
    }

    /** Push hits based on uris */
    async pushUriHit() {
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
        const messageDiv = document.getElementById("sublessMessage");
        const link = document.createElement("a");
        const img = document.createElement("img");
        const urls = this.getmessage();
        img.src = urls[0];
        img.id = "sublessMessageImage";
        img.style.maxHeight = "90px";
        link.href = urls[1];
        link.id = "sublessMessageLink";
        link.appendChild(img);
        if (messageDiv) {
            messageDiv.appendChild(link);
        }
    }

    /** Gets a random message ad and corresponding link
     * @return {[string, string]}tuple of image and target URI
    */
    private getmessage(): [string, string] {
        const message = Math.floor(Math.random() * 8) + 1;
        const img = `${sublessUri}/dist/assets/message${message}.png`;
        if (message == 3) {
            return [img, `https://www.subless.com/hf-creator-instructions?utm_campaign=message${message}`];
        }
        return [img, `https://www.subless.com/patron?utm_campaign=message${message}`];
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
                (requestAnimationFrame(tick)) || setTimeout(tick, 16);
            }
        };
        tick();
    }

    /** A method that can be used to log a user out from Subless */
    async sublessLogout() {
        window.open(sublessUri + "/bff/logout?returnUrl=" + clientBaseUri, "_blank");
    }

    /** Get all subless tagged content */
    async getSublessTags(): Promise<string[]> {
        const creators: string[] = [];
        const sublessTags = document.querySelectorAll("subless");
        for (let i = 0; i < sublessTags.length; i++) {
            if (sublessTags[i] instanceof HTMLElement) {
                const tag = sublessTags[i].getAttribute("creatorName");
                if (tag) {
                    creators.push(tag);
                }
            }
        }
        return creators;
    }
}

/** Returns a subess instance with URI hit tracking
 * @return {Subless} a subless instance
*/
export function SublessUsingUris() {
    return new Subless(HitStrategy.uri);
}


/** Returns a subess instance with tag hit tracking
 * @return {Subless} a subless instance
*/
export function SublessUsingTags() {
    return new Subless(HitStrategy.tag);
}