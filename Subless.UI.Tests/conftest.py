import os

import pytest
from selenium import webdriver
from webdriver_manager.chrome import ChromeDriverManager
from webdriver_manager.firefox import GeckoDriverManager

from ApiLib import Admin
from EmailLib.MailSlurp import get_or_create_inbox
from PageObjectModels.LoginPage import LoginPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage
from UsersLib.Users import get_user_id_and_cookie

print(os.getcwd())

def pytest_addoption(parser):
    parser.addoption("--password", action="store", help="override login password for all tests")
    parser.addoption("--environment", action="store", default='dev', help="dev/test/pay")


@pytest.fixture(autouse=True)
def params(request):
    params = {'password': request.config.getoption('--password') \
        if request.config.getoption('--password') is not None \
        else 'SublessGodUser'}
    if params['password'] is None:
        pytest.skip('No valid password available for tests')

    params['environment'] = request.config.getoption('--environment')
    os.environ["environment"] = request.config.getoption(
        '--environment')  # set an env var so we can get it outside of tests

    return params


# free mailslurp account limits you to 50 accounts per month,
# so ....
@pytest.fixture
def mailslurp_inbox():
    from EmailLib import MailSlurp

    # create
    inbox = MailSlurp.get_or_create_inbox("DisposableInbox")

    yield inbox

@pytest.fixture(scope='session')
def user_data():
    from UsersLib.Users import get_all_test_user_data, save_user_test_data
    data = get_all_test_user_data()

    yield data

    save_user_test_data(data)


# ideally this would be some sort of API call
#   but we're working with what we've got available
#   returns new user, with browser at plan selection screen
@pytest.fixture
def subless_account(mailslurp_inbox, firefox_driver, ):
    from UsersLib.Users import create_user

    mailbox = get_or_create_inbox('DisposableInbox')
    attempt_to_delete_user(firefox_driver, mailbox)

    # create
    id, cookie = create_user(firefox_driver, mailbox)
    yield id, cookie

    # LoginPage(firefox_driver).logout()  # do we need to logout here??

    # HACK: delete user
    # I hate this.
    attempt_to_delete_user(firefox_driver, mailbox)


@pytest.fixture
def paying_user(firefox_driver, subless_account):
    plan_selection_page = PlanSelectionPage(firefox_driver)

    # WHEN: I select a plan
    stripe_signup_page = plan_selection_page.select_plan_5()

    # THEN: I should be taken to the stripe page
    dashboard = stripe_signup_page.SignUpForStripe()


def attempt_to_delete_user(firefox_driver, mailbox):
    from ApiLib import User
    try:
        resultpage = LoginPage(firefox_driver).open().sign_in(mailbox.email_address, "SublessTestUser")
        if 'terms' in firefox_driver.current_url:
            plan_selection_page = resultpage.accept_terms()
        id, cookie = get_user_id_and_cookie(firefox_driver)
        User.delete_user(cookie)
    except BaseException as err:  # awful.
        return

# this is technically also a fixture!
# in the cases where we need two distinct users
# additional_subless_account = subless_account
# TODO:  nope, can't do this until mailslurp month rolls over


@pytest.fixture
def subless_admin_account(subless_god_account):
    from ApiLib import User
    from UsersLib.Users import create_user
    god_id, god_cookie = subless_god_account

    mailbox = get_or_create_inbox('AdminUser')
    # create
    id, cookie = create_user(firefox_driver, mailbox)
    Admin.set_admin(id, god_cookie)
    # LoginPage(firefox_driver).logout()  # do we need to logout here??

    yield id, cookie

    User.delete_user(cookie)


@pytest.fixture
def subless_partner_account():
    from ApiLib import User
    from UsersLib.Users import create_user

    mailbox = get_or_create_inbox('PartnerUser')
    # create
    id, cookie = create_user(firefox_driver, mailbox)
    # LoginPage(firefox_driver).logout()  # do we need to logout here??
    # TODO:  set partner perms

    yield id, cookie

    User.delete_user(cookie)


@pytest.fixture
def subless_unactivated_creator_user(firefox_driver):
    from ApiLib import User
    from UsersLib.Users import create_from_login_page
    from PageObjectModels.TestSite.TestSite_HomePage import TestSite_HomePage
    mailbox = get_or_create_inbox('CreatorUser')
    # cleanup
    attempt_to_delete_user(firefox_driver, mailbox)

    # create
    test_site = TestSite_HomePage(firefox_driver)
    test_site.open()
    profile_page = test_site.click_profile()
    profile_page.click_activate()

    id, cookie = create_from_login_page(firefox_driver, mailbox)

    yield id, cookie

    User.delete_user(cookie)

@pytest.fixture
def subless_activated_creator_user(firefox_driver, subless_unactivated_creator_user):
    from PageObjectModels.PayoutSetupPage import PayoutSetupPage
    mailbox = get_or_create_inbox('CreatorUser')

    payout_page = PayoutSetupPage(web_driver)
    payout_page.enter_creator_paypal(mailbox.email_address)
    payout_page.submit_creator_paypal()


@pytest.fixture
def subless_god_account(user_data, firefox_driver):
    from Keys.Keys import Keys

    login_page = LoginPage(firefox_driver).open()
    login_page.sign_in(Keys.god_email, Keys.god_password)

    id, cookie = get_user_id_and_cookie(firefox_driver)

    # login_page.logout()  # do we need to logout here??

    user_data['GodUser'] = {'id': id,
                            'email': Keys.god_email,
                            'cookie': cookie}

    yield id, cookie


@pytest.fixture(params=[
    # 'chrome_driver',
    'firefox_driver',
])
def web_driver(request):
    return request.getfixturevalue(request.param)


@pytest.fixture
def chrome_driver():
    driver = webdriver.Chrome(ChromeDriverManager().install())
    driver.set_page_load_timeout(15)
    yield driver
    driver.close()


@pytest.fixture
def firefox_driver():
    driver = webdriver.Firefox(executable_path=GeckoDriverManager().install())
    driver.set_page_load_timeout(15)
    yield driver
    driver.close()