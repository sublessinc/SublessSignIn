import json
import os

import mailslurp_client
import re

from Keys.Keys import Keys

configuration = mailslurp_client.Configuration()
configuration.api_key['x-api-key'] = Keys.mailslurp_api_key


# CURRENTLY DEFINED MAILSLURP USERS
# GodUser
# AdminUser
# PartnerUser
# CreatorUser
# BasicUser

CreatorInbox = "CreatorUser"
PatronInbox = "DisposableInbox"

def get_or_create_inbox(inbox_name):
    inboxes = get_inboxes_from_name(inbox_name)
    if any(inboxes):
        return inboxes[0]
    return create_inbox(inbox_name)


def create_inbox(name):
    with mailslurp_client.ApiClient(configuration) as api_client:

        # create an inbox using the inbox controller
        api_instance = mailslurp_client.InboxControllerApi(api_client)
        if not name:
            inbox = api_instance.create_inbox()
        else:
            inbox = api_instance.create_inbox(name=name)
        return inbox


def delete_inbox_by_id(inbox_id):
    with mailslurp_client.ApiClient(configuration) as api_client:
        # create an inbox using the inbox controller
        api_instance = mailslurp_client.InboxControllerApi(api_client)
        api_instance.delete_inbox(inbox_id)


def get_inboxes_from_name(inbox_name):
    with mailslurp_client.ApiClient(configuration) as api_client:
        # create an inbox using the inbox controller
        inbox_controller = mailslurp_client.InboxControllerApi(api_client)
        inboxes = inbox_controller.get_all_inboxes(page=0)
        mailslurp_client.EmailControllerApi(api_client).delete_all_emails() # todo: should I be doing this?

        inboxes = list((x for x in inboxes.content if x.name == inbox_name))

        return inboxes


def receive_email(inbox_name='', inbox_id=''):
    if not inbox_id and not inbox_name:
        raise Exception('Must specify either an inbox name or an inbox id')

    with mailslurp_client.ApiClient(configuration) as api_client:
        if not inbox_id:
            inbox_id = get_or_create_inbox(inbox_name)

        # wait for email
        wait_controller = mailslurp_client.WaitForControllerApi(api_client)
        email = wait_controller.wait_for_latest_email(inbox_id=inbox_id,
                                                      timeout=30000, unread_only=True)

        return email


def get_newest_otp(inbox_name='', inbox_id=''):
    email_body = receive_email(inbox_name=inbox_name, inbox_id=inbox_id).body
    matches = re.findall(r'\d+', email_body)

    return matches[0]
