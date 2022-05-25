import os

from EmailLib import MailSlurp
from PageObjectModels.LoginPage import LoginPage
from EmailLib.MailSlurp import get_inbox_from_name
from PageObjectModels.PlanSelectionPage import PlanSelectionPage


def test_first_time_flow(firefox_driver, subless_account, params):
    print(os.getcwd())
    # GIVEN: I am on the Subless login page, as a completely new user

    # create
    plan_selection_page = PlanSelectionPage(firefox_driver)  # works under the assumption that fixture "subless_account" leaves us on plan selection
    # THEN: I should be taken to the plan selection page
    assert "subless" in firefox_driver.title
    assert 'register-payment' in firefox_driver.current_url
    stripe_signup_page = plan_selection_page.select_plan()
    stripe_signup_page.SignUpForStripe()


    # AND: I should be able to successfully log out
    plan_selection_page.logout()
    assert 'login' in firefox_driver.current_url
