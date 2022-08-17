import logging
import json

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

test_url = "https://dev.subless.com/api/Checkout/create-checkout-session"


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

    # create 1000 identical requests
    urls = []
    for i in range(20):
        urls.append(test_url)
    reqs = (grequests.post(url, headers=headers, data=payload) for url in urls)

    # execute the requests 100 at a time
    captured_status_codes = []
    for resp in grequests.imap(reqs, size=5):
        captured_status_codes.append(resp.status_code)

    assert all([status_code == 200 for status_code in captured_status_codes])
