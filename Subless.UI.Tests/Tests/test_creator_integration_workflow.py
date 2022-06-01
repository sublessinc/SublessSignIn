from EmailLib.MailSlurp import get_or_create_inbox, receive_email, CreatorInbox, PatronInbox
from PageObjectModels.PayoutSetupPage import PayoutSetupPage
from UsersLib.Users import DefaultPassword


def test_creator_activate(web_driver, subless_unactivated_creator_user, params):
    # WHEN: I sign up for a creator account
    # THEN: I am prompted to provide payout details
    mailbox = get_or_create_inbox(CreatorInbox)
    assert "payout" in web_driver.current_url
    payout_page = PayoutSetupPage(web_driver)
    payout_page.enter_creator_paypal(mailbox.email_address)
    # AND: I should receive an email after submitting
    payout_page.submit_creator_paypal()
    email = receive_email(inbox_id=mailbox.id)
    assert "Subless payout email set" in email.subject
    assert "TestUser" in email.body




def test_creator_hit(web_driver, subless_activated_creator_user, paying_user, params):
    # WHEN a creator with a set number of hits is visited by a patron
    patron_mailbox = get_or_create_inbox(PatronInbox)
    creator_mailbox = get_or_create_inbox(CreatorInbox)
    from PageObjectModels.LoginPage import LoginPage
    # Login creator
    login_page = LoginPage(web_driver).open()
    creator_dashboard = login_page.sign_in(creator_mailbox.email_address, DefaultPassword)
    assert "profile" in web_driver.current_url
    # get current hits
    before_hit_count = creator_dashboard.get_hit_count()
    creator_dashboard.logout()
    login_page = LoginPage(web_driver).open()
    patron_dashboard = login_page.sign_in(patron_mailbox.email_address, DefaultPassword)
    from PageObjectModels.TestSite.TestSite_HomePage import TestSite_HomePage
    test_site = TestSite_HomePage(web_driver).open()
    test_site.click_profile()
    # THEN the creator's hit count should increase by one
    login_page = LoginPage(web_driver).open()
    creator_dashboard = login_page.sign_in(creator_mailbox.email_address, DefaultPassword)
    after_hit_count = creator_dashboard.get_hit_count()
    assert after_hit_count-before_hit_count == 1


