import json


class Keys:
    all_json = json.load(open('../keys.json'))

    mailslurp_api_key = all_json['mailslurp_api_key']
    god_token = all_json['god_token']
    god_password = all_json['god_password']
    god_email = all_json['god_email']

