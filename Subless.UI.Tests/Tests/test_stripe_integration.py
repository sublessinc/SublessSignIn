import logging
import json

from Keys.Keys import Keys

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

test_url = f"https://{Keys.subless_uri}/api/Checkout/create-checkout-session"


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

    # create many identical requests
    urls = []
    for i in range(100):
        urls.append(test_url)
    reqs = (grequests.post(url, headers=headers, data=payload) for url in urls)

    # execute the requests in batches of SIZE
    captured_status_codes = []
    for resp in grequests.imap(reqs, size=20):
        if (resp.status_code!=200):
            logger.error(f"Request failed {resp.status_code} : {resp}")
        captured_status_codes.append(resp.status_code)

    assert all([status_code == 200 for status_code in captured_status_codes])
