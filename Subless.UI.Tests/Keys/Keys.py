import json
import os


class Keys:

    mailslurp_api_key = os.environ['mailslurp_api_key']
    # I assume kyle is referring to us switching from jwt based authentication to cookies that exist for a session
    # lifetime. That probably will require some re-vetting of assumptions whenever I find where this was used.
    god_password = os.environ['god_password']
    god_email = os.environ['god_email']
    cognito_app_client_id = os.environ['cognito_app_client_id']
