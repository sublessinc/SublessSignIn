import { Log, UserManagerSettings } from "oidc-client-ts";

export function foo() {
    var foo: UserManagerSettings = {
        response_type: "code",
        authority: "foo",
        redirect_uri: "barr",
        client_id: "baz"
    }
    Log.logger = window.console;
    Log.level = Log.INFO;
    console.warn("Working???" + foo.redirect_uri);
}

//export default foo;
//# sourceMappingURL=test.js.map