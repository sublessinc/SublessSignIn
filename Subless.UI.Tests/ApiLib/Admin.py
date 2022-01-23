import json
import os

import requests


def set_admin(user_id, cookie):
    url = f'https://{os.environ["environment"]}.subless.com/api/Admin/setadmin?userId={user_id}'

    payload = {}
    headers = {
        'subless': f'{auth_token}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'SetAdmin returned code: {response.status_code} - {response.reason} - {response.text}')


def get_user_capabilities(user_id, cookie):
    url = f'https://{os.environ["environment"]}.subless.com/api/Admin/userCapabilties?userId={user_id}'

    payload = {}
    headers = {
        'subless': f'{auth_token}'
    }

    response = requests.request("GET", url, headers=headers, data=payload)

    print(f'GetUserCapabilities returned code: {response.status_code} - {response.reason} - {response.text}')

    return json.loads(response.content)
