import json
import logging
import os
import time

import pytest
import simplejson
from mailslurp_client import ApiException

from ApiLib.User import delete_user_by_email
from EmailLib import MailSlurp
from EmailLib.MailSlurp import PatronInbox, receive_email
from Exceptions.Exceptions import ApiLimitException, ExistingUserException
from PageObjectModels.BasePage import BasePage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

DefaultPassword = 'SublessTestUser'

def create_user(driver, inbox):
    from PageObjectModels.LoginPage import LoginPage
    login_page = LoginPage(driver).open()
    return create_from_login_page(driver, inbox)


def create_from_login_page(driver, inbox):
    from PageObjectModels.LoginPage import LoginPage
    login_page = LoginPage(driver)
    sign_up_page = login_page.click_sign_up()
    assert "signup" in driver.current_url
    time.sleep(5)
    try:
        otp_page = sign_up_page.sign_up(inbox.email_address,
                                    DefaultPassword)
    except ExistingUserException:
        attempt_to_delete_user(inbox.email_address, DefaultPassword)
        raise Exception("Test failed due to existing user, possible cleanup failure")
    except ApiLimitException:
        # We should clean up the user that just got hosed by a rate limit
        delete_locked_user(driver, inbox.email_address)
        raise Exception("Test failed due to API rate limit")
    otp =  MailSlurp.get_newest_otp(inbox_id=inbox.id)
    try:
        terms_page = otp_page.confirm_otp(otp)
    except ApiException:
        # OTP never arrived, should probably nuke the account
        delete_locked_user(driver, inbox.email_address)
        raise Exception("Test failed due to never receiving OTP")
    plan_selection_page = terms_page.accept_terms()
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

def create_subless_account(web_driver):
    from UsersLib.Users import create_user
    from EmailLib.MailSlurp import get_or_create_inbox

    mailbox = get_or_create_inbox(PatronInbox)

    # create
    id, cookie = create_user(web_driver, mailbox)
    return id, cookie, mailbox

def create_paid_subless_account(web_driver):
    from PageObjectModels.PlanSelectionPage import PlanSelectionPage
    from EmailLib.MailSlurp import get_or_create_inbox

    id, cookie, mailbox = create_subless_account(web_driver)
    plan_selection_page = PlanSelectionPage(web_driver)

    # WHEN: I select a plan
    stripe_signup_page = plan_selection_page.select_plan_5()

    # THEN: I should be taken to the stripe page
    dashboard = stripe_signup_page.sign_up_for_stripe()
    welcome_email = receive_email(inbox_id=mailbox.id)
    return id, cookie, mailbox

def select_plan_for_subless_account(web_driver, mailbox):
    from PageObjectModels.PlanSelectionPage import PlanSelectionPage
    plan_selection_page = PlanSelectionPage(web_driver).open()

    # WHEN: I select a plan
    stripe_signup_page = plan_selection_page.select_plan_5()

    # THEN: I should be taken to the stripe page
    dashboard = stripe_signup_page.sign_up_for_stripe()
    welcome_email = receive_email(inbox_id=mailbox.id)


def create_unactivated_creator_User(web_driver, mailbox):
    from UsersLib.Users import create_from_login_page
    from PageObjectModels.TestSite.TestSite_LoginPage import TestSiteLoginPage

    # cleanup
    # let's remove this for now since we manually unlock on failures
    # attempt_to_delete_user(web_driver, mailbox)

    # create
    test_site = TestSiteLoginPage(web_driver)
    test_site.open()
    profile_page = test_site.click_profile()
    profile_page.click_activate()

    return create_from_login_page(web_driver, mailbox)

def create_activated_creator_user(web_driver, mailbox):
    from PageObjectModels.PayoutSetupPage import PayoutSetupPage

    id, cookie = create_unactivated_creator_User(web_driver, mailbox)
    payout_page = PayoutSetupPage(web_driver)
    payout_page.enter_creator_paypal(mailbox.email_address)
    payout_page.submit_creator_paypal()
    return id, cookie

def attempt_to_delete_user(firefox_driver, mailbox):
    from PageObjectModels.LoginPage import LoginPage
    from ApiLib import User
    try:
        logging.info("Deleting user")
        login = LoginPage(firefox_driver).open()
        resultpage = login.sign_in(mailbox.email_address, DefaultPassword)
        if 'terms' in firefox_driver.current_url:
            plan_selection_page = resultpage.accept_terms()
        page = BasePage(firefox_driver)
        account_settings = page.navigate_to_account_settings()


        # THEN: I should have the ability to cancel that plan
        login_page = account_settings.delete_account()
        # AND: I should be prompted to login
        assert "login" in firefox_driver.current_url
    except BaseException as err:  # awful.
        return


def login_as_god_user(firefox_driver):
    from Keys.Keys import Keys
    from PageObjectModels.LoginPage import LoginPage

    login_page = LoginPage(firefox_driver).open()
    login_page.sign_in(Keys.god_email, Keys.god_password)

    id, cookie = get_user_id_and_cookie(firefox_driver)
    return id, cookie, login_page

def delete_locked_user(firefox_driver, email):
    id,cookie,login_page = login_as_god_user(firefox_driver)
    delete_user_by_email(cookie, email)

