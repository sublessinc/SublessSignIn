var baseURI = location.protocol + '//' + window.location.hostname + (location.port ? ':' + location.port : '');
var redirectURI = baseURI + "/index.html";

var bindLogout = function (settings) {
    document
        .getElementById("logout")
        .addEventListener("click", function (evt) {
            window.location.replace(
                settings.issuerUrl
                + "/logout?response_type=code&client_id="
                + settings.appClientId
                + "&logout_uri="
                + redirectURI
            );
        });
}

fetch("/api/Authorization/settings")
    .then(function (resp) {
        var json = resp.json().then(json => {
            bindLogout(json);
        });
    });