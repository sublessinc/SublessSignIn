import json
import os

import requests


def set_admin(user_id, cookie):
    url = f'https://{os.environ["environment"]}.subless.com/api/Admin/setadmin?userId={user_id}'

    payload = {}
    headers = {
        'subless': f'{cookie}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'SetAdmin returned code: {response.status_code} - {response.reason} - {response.text}')


def get_user_capabilities(user_id, cookie):
    url = f'https://{os.environ["environment"]}.subless.com/api/Admin/userCapabilties?userId={user_id}'

    payload = {}
    headers = {
        'subless': f'{cookie}'
    }

    response = requests.request("GET", url, headers=headers, data=payload)

    print(f'GetUserCapabilities returned code: {response.status_code} - {response.reason} - {response.text}')

    return json.loads(response.content)


def get_payout_calculation( cookie, start, end):
    url = f'https://{os.environ["environment"]}.subless.com/api/Calculator?start={start}&end={end}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("GET", url, headers=headers, data=payload)

    print(f'Calculator returned code: {response.status_code} - {response.reason} - {response.text}')

    return json.loads(response.content)

def execute_payout( cookie, start, end):
    url = f'https://{os.environ["environment"]}.subless.com/api/Calculator?start={start}&end={end}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'Calculator returned code: {response.status_code} - {response.reason} - {response.text}')