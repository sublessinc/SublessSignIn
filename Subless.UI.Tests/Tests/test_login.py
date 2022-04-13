import pytest
import time
import os

from EmailLib import MailSlurp
#from Fixtures.drivers import *
from PageObjectModels.LoginPage import LoginPage
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


def test_new_un_pass_login(web_driver, params):
    # GIVEN: I am on the Subless login page, as a new user
    login_page = LoginPage(web_driver).open()

    # WHEN: I enter my username and password and click 'sign in'
    plan_selection_page = login_page.sign_in(
        'sublesstestuser+unselected@gmail.com', params['password'])

    # THEN: I should be taken to the plan selection page
    assert "Sublessui" in web_driver.title
    assert 'register-payment' in web_driver.current_url

    # AND: I should be able to successfully log out
    plan_selection_page.logout()
    assert 'login' in web_driver.current_url


@pytest.mark.skip("not yet implemented")
def test_new_google_login(web_driver, params):
    # GIVEN: I am on the Subless login page, as a new user

    # WHEN: I sign in through my google account

    # THEN: I should be taken to the plan selection page

    # AND: I should be able to successfully log out

    pass


@pytest.mark.skip("not yet implemented")
def test_incomplete_payment_processing(web_driver, params):
    # GIVEN: I am on the Subless login page, as a user who did not complete billing set up

    # WHEN: I sign in with my username and password

    # THEN: I should be taken to the plan selection page

    # AND: I should be able to successfully log out
    pass


def test_user_creation(web_driver, mailslurp_inbox, params):
    # GIVEN: I am on the Subless login page, as a completely new user
    login_page = LoginPage(web_driver).open()

    # WHEN: I create a new account
    sign_up_page = login_page.click_sign_up()
    assert "signup" in web_driver.current_url

    otp_page = sign_up_page.sign_up(mailslurp_inbox.email_address,
                                    'SublessTestUser')
    plan_selection_page = otp_page.confirm_otp(
        MailSlurp.get_newest_otp(inbox_id=mailslurp_inbox.id))

    # THEN: I should be taken to the plan selection page
    assert "Sublessui" in web_driver.title
    assert 'register-payment' in web_driver.current_url

    # AND: I should be able to successfully log out
    plan_selection_page.logout()
    assert 'login' in web_driver.current_url


# todo: figure out this workflow
@pytest.mark.skip
def test_forgot_password(web_driver, params):
    pass
