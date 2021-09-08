
interface Subless {
    async subless_GetConfig(): Promise<UserManagerSettings>;
    async subless_initUserManagement(): Promise<UserManager>;
    async subless_startLogin(): Promise<void>;
    async sublessLogin(): Promise<void>;
    async subless_LoggedIn(): Promise<void>;
    async subless_loginCallback(): Promise<void>;
    async subless_handleCallback(): Promise<void>;
    async subless_hit(): Promise<void>;
    async sublessLogout(): Promise<void>;
    async subless_revoke(): Promise<void>;
}
