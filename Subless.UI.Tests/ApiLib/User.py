import os

import requests

from Keys.Keys import Keys

def delete_user(cookie):
    url = f'https://{Keys.subless_uri}/api/User'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }
    print(url)
    print(payload)
    print(headers)

    response = requests.request("DELETE", url, headers=headers, data=payload, verify=False)

    print(f'Delete returned code: {response.status_code} - {response.reason} - {response.text}')

def delete_user_by_email(cookie, email):
    url = f'https://{Keys.subless_uri}/api/User/byemail?email={email}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }
    print(url)
    print(payload)
    print(headers)

    response = requests.request("DELETE", url, headers=headers, data=payload, verify=False)

    print(f'Delete returned code: {response.status_code} - {response.reason} - {response.text}')
