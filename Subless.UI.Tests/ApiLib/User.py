import os

import requests


def delete_user(cookie):
    url = f'https://{os.environ["environment"]}.subless.com/api/User'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }
    print(url)
    print(payload)
    print(headers)

    response = requests.request("DELETE", url, headers=headers, data=payload)

    print(f'Delete returned code: {response.status_code} - {response.reason} - {response.text}')
