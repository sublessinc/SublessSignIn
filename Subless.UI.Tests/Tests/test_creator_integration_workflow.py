from EmailLib.MailSlurp import get_or_create_inbox, receive_email
from PageObjectModels.PayoutSetupPage import PayoutSetupPage


def test_creator_activate(web_driver, subless_unactivated_creator_user, params):
    # WHEN: I sign up for a creator account
    # THEN: I am prompted to provide payout details
    mailbox = get_or_create_inbox('CreatorUser')
    assert "payout" in web_driver.current_url
    payout_page = PayoutSetupPage(web_driver)
    payout_page.enter_creator_paypal(mailbox.email_address)
    # AND: I should receive an email after submitting
    payout_page.submit_creator_paypal()
    email = receive_email(inbox_id=mailbox.id)
    assert "Subless payout email set" in email.subject
    assert "TestUser" in email.body




def test_creator_hit(web_driver, subless_activated_creator_user, params):
    # WHEN: I sign up for a creator account
    # THEN: I am prompted to provide payout details
    assert "subless" not in web_driver.current_url




