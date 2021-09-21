import requests
import json
from Keys.Keys import Keys


def register_partner(user_id, auth_token):
    url = "https://dev.subless.com/api/Partner"

    payload = json.dumps({
        "cognitoAppClientId": Keys.cognito_app_client_id,
        "payPalId": "testPayoneer",
        "site": "https://pythonclientdev.subless.com",
        "userPattern": "https://pythonclientdev.subless.com/profile/creator",
        "admin": user_id
    })
    headers = {
        'Content-Type': 'application/json',
        'Authorization': f'Bearer {auth_token}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'RegisterPartner returned code: {response.status_code} - {response.reason} - {response.text}')

