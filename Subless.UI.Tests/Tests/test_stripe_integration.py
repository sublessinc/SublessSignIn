import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

url = "https://dev.subless.com/api/Authorization/redirect"

def test1(subless_account):
    # this library must be loaded before the requests library, which is done in conftest.py
    grequests = __import__("grequests")
    headers = {
        'Activation': 'consectetur',
        'Cookie': f'subless={subless_account[1]}'
    }
    reqs = []
    # create 200 identical requests
    for i in range(1000):
        reqs.append(grequests.get(url, headers=headers))

    # execute the requests 20 at a time
    for resp in grequests.imap(reqs, size=100):
        assert resp.status_code == 200
