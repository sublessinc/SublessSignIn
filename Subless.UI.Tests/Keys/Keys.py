import json
import os


class Keys:
    keyPath = os.path.join(os.path.dirname(os.path.abspath(__file__)), '../keys.json')
    if (os.path.isfile(keyPath)):
        all_json = json.load(open(keyPath))
        mailslurp_api_key = all_json['mailslurp_api_key']
        god_token = all_json['god_token']  # ha ha this is no longer valid
        god_password = all_json['god_password']
        god_email = all_json['god_email']
        cognito_app_client_id = all_json['cognito_app_client_id']
        subless_uri = all_json['subless_uri']
        test_client_uri = all_json['test_client_uri']
        hf_uri = all_json['hf_uri']
    else:
        mailslurp_api_key = os.environ['mailslurp_api_key']
        god_password = os.environ['god_password']
        god_email = os.environ['god_email']
        cognito_app_client_id = os.environ['cognito_app_client_id']
        subless_uri = os.environ['subless_uri']
        test_client_uri = os.environ['test_client_uri']
        hf_uri = os.environ['hf_uri']

