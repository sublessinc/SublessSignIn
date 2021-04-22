const urlParams = new URLSearchParams(window.location.search);
const sessionId = urlParams.get("session_id")
let customerId;

if (sessionId) {
    fetch("/api/Checkout/checkout-session?sessionId=" + sessionId)
    .then(function(result){
      return result.json()
    })
    .then(function(session){
      // We store the customer ID here so that we can pass to the
      // server and redirect to customer portal. Note that, in practice
      // this ID should be stored in your database when you receive
      // the checkout.session.completed event. This demo does not have
      // a database, so this is the workaround. This is *not* secure.
      // You should use the Stripe Customer ID from the authenticated
      // user on the server.
      customerId = session.customer;

        var sessionJSON = JSON.stringify(session, null, 2);
        notifyUserSuccess(session);
      document.querySelector("pre").textContent = sessionJSON;
    })
    .catch(function(err){
      console.log('Error when fetching Checkout session', err);
    });

  // In production, this should check CSRF, and not pass the session ID.
  // The customer ID for the portal should be pulled from the 
  // authenticated user on the server.
  const manageBillingForm = document.querySelector('#manage-billing-form');
  manageBillingForm.addEventListener('submit', function(e) {
    e.preventDefault();
      fetch('/api/Checkout/customer-portal', {
      method: 'POST',
      headers: {
        'Content-Type': 'application/json',
      },
      body: JSON.stringify({
        sessionId: sessionId
      }),
    })
      .then((response) => response.json())
      .then((data) => {
        window.location.href = data.url;
      })
      .catch((error) => {
        console.error('Error:', error);
      });
  });
}
// Create a Checkout Session with the selected plan ID
var notifyUserSuccess = function (responseData) {
    let user = sessionStorage.getItem("user_info");
    responseData["userid"] = JSON.parse(user).sub;
    responseData["username"] = JSON.parse(user).username;
    return fetch("/api/Checkout/save-customer", {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(responseData)
    }).then(handleFetchResult);
};

var handleFetchResult = function (result) {
    if (!result.ok) {
        return result.json().then(function (json) {
            if (json.error && json.error.message) {
                throw new Error(result.url + ' ' + result.status + ' ' + json.error.message);
            }
        }).catch(function (err) {
            showErrorMessage(err);
            throw err;
        });
    }
    return result.json();
};