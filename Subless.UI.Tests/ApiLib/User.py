import requests


def delete(token):
    url = "https://dev.subless.com/api/User"

    payload = {}
    headers = {
        'Authorization': f'Bearer {token}'
    }

    response = requests.request("DELETE", url, headers=headers, data=payload)

    print(f'Delete returned code: {response.status_code} - {response.reason} - {response.text}')
