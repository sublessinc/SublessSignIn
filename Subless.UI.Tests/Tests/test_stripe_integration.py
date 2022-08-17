import logging
import json

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

url = "https://dev.subless.com/api/Checkout/create-checkout-session"

def test_concurrent_api_calls_do_not_error(subless_god_account):
    # this library must be loaded before the requests library, which is done in conftest.py
    grequests = __import__("grequests")
    headers = {
        'Content-Type': 'application/json',
        'Cookie': f'subless={subless_god_account[1]}'
    }
    payload = json.dumps({
        "desiredPrice": "10"
    })
    reqs = []
    # create 200 identical requests
    for i in range(1000):
        reqs.append(grequests.request("POST", url, headers=headers, data=payload))

    # execute the requests 100 at a time
    for resp in grequests.imap(reqs, size=100):
        print(resp)
        assert resp.status_code == 200, f'Expected 200, got {resp.status_code}'


