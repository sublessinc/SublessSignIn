import json
import os


class Keys:
    all_json = json.load(open(os.path.join(os.path.dirname(os.path.abspath(__file__)), '../keys.json')))

    mailslurp_api_key = all_json['mailslurp_api_key']
    god_token = all_json['god_token']  # ha ha this is no longer valid
    god_password = all_json['god_password']
    god_email = all_json['god_email']
    cognito_app_client_id = all_json['cognito_app_client_id']
