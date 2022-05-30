import time

import pytest
from selenium.webdriver.remote.webelement import WebElement

from EmailLib import MailSlurp
from PageObjectModels.LoginPage import LoginPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage
from PageObjectModels.DashboardPage import DashboardPage

def test_paying_user_directed_to_dashboard(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    # THEN: I should see my dashboard
    dashboard = DashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    # AND: I should be able to successfully log out
    dashboard.logout()
    assert 'login' in web_driver.current_url

def test_paying_user_can_view_plan(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    # THEN: I should see my dashboard
    dashboard = DashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    plan_select = dashboard.navigate_to_change_plan()
    assert "plan" in web_driver.current_url
    time.sleep(3)
    assert plan_select.is_5_plan_selected == "true"
    manage_page = plan_select.navigate_to_billing()
    original_window = web_driver.current_window_handle
    for window_handle in web_driver.window_handles:
        if window_handle != original_window:
            web_driver.switch_to.window(window_handle)
            break
    assert "stripe" in web_driver.current_url
    plan = manage_page.current_plan_text
    assert plan.text == "$5.00 per month"
    web_driver.close()
    # Switch back to the old tab or window
    web_driver.switch_to.window(original_window)


def test_paying_user_can_cancel_plan(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    # THEN: I should see my dashboard
    dashboard = DashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    plan_select = dashboard.navigate_to_change_plan()
    assert "plan" in web_driver.current_url
    time.sleep(3)
    assert plan_select.is_5_plan_selected == "true"
    plan_select.change_plan_10()
    web_driver.refresh()
    time.sleep(3)
    assert plan_select.is_5_plan_selected == "false"
    assert plan_select.is_10_plan_selected == "true"
    manage_page = plan_select.navigate_to_billing()
    original_window = web_driver.current_window_handle
    for window_handle in web_driver.window_handles:
        if window_handle != original_window:
            web_driver.switch_to.window(window_handle)
            break
    assert "stripe" in web_driver.current_url
    plan = manage_page.current_plan_text
    assert plan.text == "$10.00 per month"
    web_driver.close()
    # Switch back to the old tab or window
    web_driver.switch_to.window(original_window)
