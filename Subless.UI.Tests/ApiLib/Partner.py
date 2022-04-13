import os

import requests
import json
from Keys.Keys import Keys


def register_partner(user_id, cookie,
                     paypal_id='testPayoneer',
                     site='https://pythonclientdev.subless.com',
                     user_pattern='https://pythonclientdev.subless.com/profile/creator'):
    url = f'https://{os.environ["environment"]}.subless.com/api/Partner'

    payload = json.dumps({
        "cognitoAppClientId": Keys.cognito_app_client_id,
        "payPalId": paypal_id,
        "site": site,
        "userPattern": user_pattern,
        "admin": user_id
    })
    headers = {
        'Content-Type': 'application/json',
        'Cookie': f'subless={cookie}'
    }

    response = requests.request("POST", url, headers=headers, data=payload)

    print(f'RegisterPartner returned code: {response.status_code} - {response.reason} - {response.text}')
