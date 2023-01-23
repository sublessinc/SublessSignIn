import datetime
import logging
import time

import pytest
from dateutil.relativedelta import relativedelta
from selenium.webdriver.remote.webelement import WebElement

from EmailLib import MailSlurp
from PageObjectModels.LoginPage import LoginPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage
from PageObjectModels.PatronDashboardPage import PatronDashboardPage
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

def test_paying_user_directed_to_dashboard(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    # THEN: I should see my dashboard
    dashboard = PatronDashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    # AND: I should be able to successfully log out
    dashboard.logout()
    assert 'login' in web_driver.current_url

def test_paying_user_can_view_plan(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    dashboard = PatronDashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    plan_select = dashboard.navigate_to_change_plan()
    assert "plan" in web_driver.current_url
    logging.info("Waiting for plan page to render")
    time.sleep(3)
    # THEN: My plan should be visible on the plan select page
    assert plan_select.is_5_plan_selected == "true"

    # AND: My plan should be $5 on the stripe site
    manage_page = plan_select.navigate_to_billing()
    time.sleep(3)
    original_window = web_driver.current_window_handle
    for window_handle in web_driver.window_handles:
        if window_handle != original_window:
            web_driver.switch_to.window(window_handle)
            break
    assert "stripe" in web_driver.current_url
    time.sleep(3)
    plan = manage_page.current_plan_text
    assert plan.text == "$5.00 per month"
    web_driver.close()
    # Switch back to the old tab or window
    web_driver.switch_to.window(original_window)


def test_paying_user_can_change_plan(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    # THEN: I should see my dashboard
    dashboard = PatronDashboardPage(web_driver)
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
    time.sleep(3)
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


def test_paying_user_can_cancel_plan(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    dashboard = PatronDashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    plan_select = dashboard.navigate_to_change_plan()
    assert "plan" in web_driver.current_url
    time.sleep(3)
    assert plan_select.is_5_plan_selected == "true"
    account_settings = plan_select.navigate_to_account_settings()
    # THEN: I should have the ability to cancel that plan
    login_page = account_settings.cancel_subscription()
    # AND: I should not be logged out
    assert "account-settings" in web_driver.current_url


def test_cancelled_user_sees_warning(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    dashboard = PatronDashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url

    plan_select = dashboard.navigate_to_change_plan()
    assert "plan" in web_driver.current_url
    time.sleep(3)
    assert plan_select.is_5_plan_selected == "true"
    account_settings = plan_select.navigate_to_account_settings()
    # THEN: I should have the ability to cancel that plan
    login_page = account_settings.cancel_subscription()
    # AND: I should not be logged out
    assert "account-settings" in web_driver.current_url
    dashboard = PatronDashboardPage(web_driver).open()
    web_driver.refresh()
    dashboard.wait_for_load()
    cancel_date = datetime.date.today() + relativedelta(months=1)
    time.sleep(1) # wait for api call
    assert str(cancel_date.day) in dashboard.cancellation_message.text
    assert str(cancel_date.year) in dashboard.cancellation_message.text

def test_paying_user_can_delete_account(web_driver, paying_user, params):
    # WHEN: I sign up and pay for an account
    dashboard = PatronDashboardPage(web_driver)
    assert "subless" in web_driver.title
    assert 'profile' in web_driver.current_url
    account_settings = dashboard.navigate_to_account_settings()
    # THEN: I should have the ability to cancel that plan
    login_page = account_settings.delete_account()
    # AND: I should be prompted to login
    assert "login" in web_driver.current_url