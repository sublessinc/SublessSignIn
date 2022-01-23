import os

import requests


def delete(auth_token):
    url = f'https://{os.environ["environment"]}.subless.com/api/User'

    payload = {}
    headers = {
        'subless': f'{auth_token}'
    }

    response = requests.request("DELETE", url, headers=headers, data=payload)

    print(f'Delete returned code: {response.status_code} - {response.reason} - {response.text}')
