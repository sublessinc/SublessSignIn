import logging
from datetime import datetime, timedelta
import time

from ApiLib.Admin import execute_idle_email
from EmailLib.MailSlurp import get_or_create_inbox, PatronInbox, receive_email
from Keys.Keys import Keys
from UsersLib.Users import DefaultPassword, login_as_god_user, select_plan_for_subless_account
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

def test_user_with_no_hits_in_payout_range_recieves_idle_email(web_driver, subless_activated_creator_user, subless_account, params):
    # signin as a non-paying subless user
    patron_mailbox = get_or_create_inbox(PatronInbox)
    from PageObjectModels.LoginPage import LoginPage
    login_page = LoginPage(web_driver).open()
    patron_dashboard = login_page.sign_in(patron_mailbox.email_address, DefaultPassword)

    # register payment method for the user
    period_start = datetime.utcnow();
    select_plan_for_subless_account(web_driver, patron_mailbox)
    period_end = datetime.utcnow() + timedelta(seconds=30)

    # execute the idle email
    id, cookie, page = login_as_god_user(web_driver)
    execute_idle_email(cookie, period_start, period_end)

    # assert the email - we only care that the email got sent and recieved
    idle_email = receive_email(inbox_id=patron_mailbox.id)
    assert "Your subless budget isn't going to any creators this month!" in idle_email.subject

def test_user_with_no_hits_in_payout_range_but_previous_hits_recieves_idle_email_with_history(web_driver, subless_activated_creator_user, subless_account, params):
    # signin as a non-paying subless user
    patron_mailbox = get_or_create_inbox(PatronInbox)
    from PageObjectModels.LoginPage import LoginPage
    login_page = LoginPage(web_driver).open()
    patron_dashboard = login_page.sign_in(patron_mailbox.email_address, DefaultPassword)

    # push a hit and capture the time; sleep for 5
    from PageObjectModels.TestSite.TestSite_LoginPage import TestSiteLoginPage
    test_site = TestSiteLoginPage(web_driver).open()
    test_site.click_uri_content()
    time.sleep(5)

    # register payment method for the user
    period_start = datetime.utcnow();
    select_plan_for_subless_account(web_driver, patron_mailbox)
    period_end = datetime.utcnow() + timedelta(seconds=30)

    # execute the idle email; time period must be after the hit but include the payment
    id, cookie, page = login_as_god_user(web_driver)
    execute_idle_email(cookie, period_start, period_end)

    # assert the email
    idle_email = receive_email(inbox_id=patron_mailbox.id)
    assert "There's one week left to support creators this month! Here's a few you've enjoyed" in idle_email.subject
    assert "<li><a href=" in idle_email.body
    assert "TestUser" in idle_email.body
    assert f"{Keys.subless_uri}" in idle_email.body