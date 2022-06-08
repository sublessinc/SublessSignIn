from datetime import datetime, timedelta
import time

from ApiLib.Admin import get_payout_calculation, execute_payout
from EmailLib.MailSlurp import get_or_create_inbox, CreatorInbox, PatronInbox, receive_email
from UsersLib.Users import DefaultPassword, login_as_god_user


def test_payout_calculation(web_driver, subless_activated_creator_user, paying_user, params):
    # WHEN a creator with a set number of hits is visited by a patron
    startTime = datetime.utcnow() + timedelta(seconds=-60)
    patron_mailbox = get_or_create_inbox(PatronInbox)
    from PageObjectModels.LoginPage import LoginPage
    login_page = LoginPage(web_driver).open()
    patron_dashboard = login_page.sign_in(patron_mailbox.email_address, DefaultPassword)
    from PageObjectModels.TestSite.TestSite_HomePage import TestSite_HomePage
    test_site = TestSite_HomePage(web_driver).open()
    test_site.click_profile()
    # THEN the creator's hit count should increase by one
    time.sleep(2)
    id, cookie = login_as_god_user(web_driver)
    endTime = datetime.utcnow()
    calculator_result = get_payout_calculation(cookie, startTime, endTime)
    payments = calculator_result['allPayouts'].values()
    assert 0.89 in payments
    assert 0.09 in payments
    assert 3.56 in payments


def test_payout_emails(web_driver, subless_activated_creator_user, paying_user, params):
    # WHEN a creator with a set number of hits is visited by a patron
    startTime = datetime.utcnow() + timedelta(seconds=-60)
    patron_mailbox = get_or_create_inbox(PatronInbox)
    creator_mailbox = get_or_create_inbox(CreatorInbox)
    from PageObjectModels.LoginPage import LoginPage
    login_page = LoginPage(web_driver).open()
    patron_dashboard = login_page.sign_in(patron_mailbox.email_address, DefaultPassword)
    from PageObjectModels.TestSite.TestSite_HomePage import TestSite_HomePage
    test_site = TestSite_HomePage(web_driver).open()
    test_site.click_profile()
    # THEN the creator's hit count should increase by one
    time.sleep(2)
    id, cookie = login_as_god_user(web_driver)
    endTime = datetime.utcnow()
    execute_payout(cookie, startTime, endTime)
    patron_receipt = receive_email(inbox_id=patron_mailbox.id)
    assert "Your subless receipt" in patron_receipt.subject
    assert "TestUser" in patron_receipt.body
    assert "pythonclientdev.subless.com" in patron_receipt.body
    assert "$3.57" in patron_receipt.body
    assert "$0.89" in patron_receipt.body






