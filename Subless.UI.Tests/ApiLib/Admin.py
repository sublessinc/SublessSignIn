import json
import os
import time
from Keys.Keys import Keys
import requests


def set_admin(user_id, cookie):
    url = f'https://{Keys.subless_uri}/api/Admin/setadmin?userId={user_id}'

    payload = {}
    headers = {
        'subless': f'{cookie}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'SetAdmin returned code: {response.status_code} - {response.reason} - {response.text}')


def get_user_capabilities(user_id, cookie):
    url = f'https://{Keys.subless_uri}/api/Admin/userCapabilties?userId={user_id}'

    payload = {}
    headers = {
        'subless': f'{cookie}'
    }

    response = requests.request("GET", url, headers=headers, data=payload)

    print(f'GetUserCapabilities returned code: {response.status_code} - {response.reason} - {response.text}')

    return json.loads(response.content)


def get_payout_calculation(cookie, start, end):
    url = f'https://{Keys.subless_uri}/api/Calculator?start={start}&end={end}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("GET", url, headers=headers, data=payload)

    print(f'Calculator returned code: {response.status_code} - {response.reason} - {response.text}')

    return json.loads(response.content)


def execute_payout(cookie, start, end):
    url = f'https://{Keys.subless_uri}/api/Calculator?start={start}&end={end}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'Calculator returned code: {response.status_code} - {response.reason} - {response.text}')


def queue_calculation(cookie, start, end):
    url = f'https://{Keys.subless_uri}/api/Calculator/QueueCalculator?start={start}&end={end}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)
    return json.loads(response.content)
    print(f'Calculator returned code: {response.status_code} - {response.reason} - {response.text}')


def get_queued_result(cookie, id):
    url = f'https://{Keys.subless_uri}/api/Calculator/GetQueuedCalculation?id={id}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("GET", url, headers=headers, data=payload)
    print(f'Calculator returned code: {response.status_code} - {response.reason} - {response.text}')
    return response.content


def queue_and_wait_for_results(cookie, start, end):
    id = queue_calculation(cookie, start, end)
    result = None
    sleep_time = 10
    poll_timeout = 10 * 60
    poll_count = 0
    while not result:
        poll_count = poll_count + 1
        time.sleep(sleep_time)
        response = get_queued_result(cookie, id)
        try:
            result = json.loads(response)
        except:
            print("waiting...")
        if poll_count * sleep_time > poll_timeout:
            raise TimeoutError("Calculation never completed")
    return result


def queue_payout(cookie, start, end):
    url = f'https://{Keys.subless_uri}/api/Calculator/QueuePayment?start={start}&end={end}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'Calculator returned code: {response.status_code} - {response.reason} - {response.text}')


def delete_user(cookie, email):
    url = f'https://{Keys.subless_uri}/api/User/byemail?email={email}'

    payload = {}
    headers = {
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("DELETE", url, headers=headers)

    print(f'Deletion returned code: {response.status_code} - {response.reason} - {response.text}')
