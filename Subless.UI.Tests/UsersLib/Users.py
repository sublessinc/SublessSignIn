import json
import os
import time

import pytest
import simplejson

from EmailLib import MailSlurp


def create_user(driver, inbox):
    from PageObjectModels.LoginPage import LoginPage
    login_page = LoginPage(driver).open()
    sign_up_page = login_page.click_sign_up()
    otp_page = sign_up_page.sign_up(inbox.email_address,
                                    'SublessTestUser')

    if 'An account with the given email already exists.' in driver.page_source:
        pytest.skip('user account already exists!')

    otp_page.confirm_otp(MailSlurp.get_newest_otp(inbox_id=inbox.id))

    id, cookie = get_user_id_and_cookie(driver)

    return id, cookie


# note: assumes you are on a page where pulling IDs is valid
def get_user_id_and_cookie(driver):
    driver.get(f'{driver.current_url}#id')
    time.sleep(1)  # seemingly need to wait a sec for both these fields to populate
    id_field = driver.find_elements_by_id('id')[0]
    id = id_field.get_attribute('value')
    cookie = list(x for x in driver.get_cookies() if x['name'] == 'subless')[0]['value']
    return id, cookie


def get_all_test_user_data():
    json_path = '../userdata.json'
    if not os.path.exists(json_path):
        open(json_path, 'w')  # create a blank file if needed
    try:
        with open(json_path) as json_file:
            data = json.load(json_file)
    except ValueError as err:
        data = {}  # if we can't parse valid json, just nuke the file? I guess?
    return data

def save_user_test_data(data):
    with open('../userdata.json', 'w') as outfile:
        string = simplejson.dumps(data, indent=4, sort_keys=True)
        outfile.write(string)