import pytest
import time
import os

from EmailLib import MailSlurp
#from Fixtures.drivers import *
from Keys.Keys import Keys
from PageObjectModels.LoginPage import LoginPage
import logging

from PageObjectModels.PlanSelectionPage import PlanSelectionPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


def test_admin_un_pass_login(web_driver, params):
    # GIVEN: I am on the Subless login page, as a new user
    login_page = LoginPage(web_driver).open()

    # WHEN: I enter my username and password and click 'sign in'
    plan_selection_page = login_page.sign_in(
        Keys.god_email, Keys.god_password)

    # THEN: I should be taken to the plan selection page
    assert "subless" in web_driver.title
    assert 'register-payment' in web_driver.current_url

    # AND: I should be able to successfully log out
    plan_selection_page.logout()
    assert 'login' in web_driver.current_url

def test_user_creation(web_driver, subless_account, params):
    # WHEN a new user is created
    # THEN the plan selection page should be shown
    plan_selection_page = PlanSelectionPage(web_driver)
    assert 'register-payment' in web_driver.current_url
    # AND: I should be able to successfully log out
    plan_selection_page.logout()
    assert 'login' in web_driver.current_url