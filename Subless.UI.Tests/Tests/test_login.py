import pytest
from Fixtures.drivers import chrome_driver
from PageObjectModels.LoginPage import LoginPage
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


def test_new_un_pass_login(chrome_driver):
    # GIVEN: I am on the Subless login page, as a new user
    login_page = LoginPage(chrome_driver).open()

    # WHEN: I enter my username and password and click 'sign in'
    plan_selection_page = login_page.sign_in('x', 'y')

    # THEN: I should be taken to the plan selection page
    assert "Sublessui" in chrome_driver.title
    assert 'register-payment' in chrome_driver.current_url

    # AND: I should be able to successfully log out
    plan_selection_page.logout()
    assert 'login' in chrome_driver.current_url


@pytest.mark.skip("not yet implemented")
def test_new_google_login(chrome_driver):
    # GIVEN: I am on the Subless login page, as a new user

    # WHEN: I sign in through my google account

    # THEN: I should be taken to the plan selection page

    # AND: I should be able to successfully log out

    pass


@pytest.mark.skip("not yet implemented")
def test_incomplete_payment_processing(chrome_driver):
    # GIVEN: I am on the Subless login page, as a user who did not complete billing set up

    # WHEN: I sign in with my username and password

    # THEN: I should be taken to the plan selection page

    # AND: I should be able to successfully log out
    pass


@pytest.mark.xfail
def test_user_creation(chrome_driver):
    # GIVEN: I am on the Subless login page, as a completely new user
    login_page = LoginPage(chrome_driver).open()

    # WHEN: I click the signup link
    sign_up_page = login_page.click_sign_up_link()

    # THEN: I should be taken to the signup page
    assert "signup" in chrome_driver.title

    # AND: I should be able to create a new account
    # todo:  pending a way to circumvent OTP confirmation
    pytest.fail("not yet implemented")
