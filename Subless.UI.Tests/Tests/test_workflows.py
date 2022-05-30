import pytest

from EmailLib import MailSlurp
from PageObjectModels.LoginPage import LoginPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage
from PageObjectModels.DashboardPage import DashboardPage

def test_first_time_flow(web_driver, paying_user, mailslurp_inbox, params):
    # WHEN: I sign up and pay for an account
    # THEN: I should see my dashboard
    dashboard = DashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    # AND: I should be able to successfully log out
    dashboard.logout()
    assert 'login' in web_driver.current_url
