
function getUserId() {
    var token = sessionStorage.getItem('id_token');
    fetch("/api/admin/user", {
        method: "GET",
        headers: {
            "Authorization": "Bearer " + token,
            "Content-Type": "application/json",
        }
    }).then(function (resp) {
        resp.text().then(function (text) {
            document.getElementById("userid").innerHTML = text;
        });
    });

}

function activateWithKey() {
    var token = sessionStorage.getItem('id_token');
    var key = document.getElementById("keyBox").value;
    fetch("/api/admin/setadminwithkey?key="+key, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + token,
            "Content-Type": "application/json",
        }
    })

}

function activateOtherUser() {
    var token = sessionStorage.getItem('id_token');
    var userid = document.getElementById("useridBox").value;
    fetch("/api/admin/setadmin?userid="+userid, {
        method: "POST",
        headers: {
            "Authorization": "Bearer " + token,
            "Content-Type": "application/json",
        }
    })
}

getUserId();