import pytest

from EmailLib import MailSlurp
from PageObjectModels.LoginPage import LoginPage


def test_first_time_flow(web_driver, mailslurp_inbox, params):
    # GIVEN: I am on the Subless login page, as a completely new user
    login_page = LoginPage(web_driver).open()

    # create
    sign_up_page = login_page.click_sign_up()
    assert "signup" in web_driver.current_url

    otp_page = sign_up_page.sign_up(mailslurp_inbox.email_address,
                                    'SublessTestUser')
    terms_page = otp_page.confirm_otp(
        MailSlurp.get_newest_otp(inbox_id=mailslurp_inbox.id))

    plan_selection_page = terms_page.accept_terms()

    # THEN: I should be taken to the plan selection page
    assert "subless" in web_driver.title
    assert 'register-payment' in web_driver.current_url
    stripe_signup_page = plan_selection_page.select_plan()

    # AND: I should be taken to the stripe page
    dashboard = stripe_signup_page.SignUpForStripe()

    # AND: I should see my dashboard
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    # AND: I should be able to successfully log out
    dashboard.logout()
    assert 'login' in web_driver.current_url
