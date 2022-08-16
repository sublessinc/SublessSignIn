import logging
import os
globals()['grequests'] = __import__('grequests')

import pytest
from selenium import webdriver
from webdriver_manager.chrome import ChromeDriverManager
from webdriver_manager.firefox import GeckoDriverManager

from ApiLib import Admin
from EmailLib.MailSlurp import get_or_create_inbox, CreatorInbox, PatronInbox
from PageObjectModels.PlanSelectionPage import PlanSelectionPage
from UsersLib.Users import get_user_id_and_cookie, create_subless_account, attempt_to_delete_user, \
    create_paid_subless_account, create_unactivated_creator_User, create_activated_creator_user, login_as_god_user
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()
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
    logging.info("Creating inbox")

    # create
    inbox = MailSlurp.get_or_create_inbox(PatronInbox)

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
    logging.info("Creating subless account")
    id, cookie, mailbox = create_subless_account(firefox_driver)
    yield id, cookie

    # HACK: delete user
    # I hate this.
    logging.info("deleting subless account")
    attempt_to_delete_user(firefox_driver, mailslurp_inbox)


@pytest.fixture
def paying_user(firefox_driver):
    logging.info("Creating paid account")
    id, cookie = create_paid_subless_account(firefox_driver)
    yield id, cookie
    logging.info("Deleting subless account")
    attempt_to_delete_user(firefox_driver, mailslurp_inbox)


# this is technically also a fixture!
# in the cases where we need two distinct users
# additional_subless_account = subless_account
# TODO:  nope, can't do this until mailslurp month rolls over


@pytest.fixture
def subless_admin_account(subless_god_account):
    from ApiLib import User
    from UsersLib.Users import create_user
    logging.info("Getting admin account")
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
    logging.info("Creating partner user")
    mailbox = get_or_create_inbox('PartnerUser')
    # create
    id, cookie = create_user(firefox_driver, mailbox)
    # LoginPage(firefox_driver).logout()  # do we need to logout here??
    # TODO:  set partner perms

    yield id, cookie

    User.delete_user(cookie)


@pytest.fixture
def subless_unactivated_creator_user(firefox_driver):
    logging.info("Making unactivated creator")
    mailbox = get_or_create_inbox(CreatorInbox)
    id, cookie = create_unactivated_creator_User(firefox_driver, mailbox)
    yield id, cookie
    attempt_to_delete_user(firefox_driver, mailbox)

@pytest.fixture
def subless_activated_creator_user(firefox_driver):
    logging.info("Making activated creator")
    mailbox = get_or_create_inbox(CreatorInbox)
    id, cookie = create_activated_creator_user(firefox_driver, mailbox)
    yield id, cookie
    attempt_to_delete_user(firefox_driver, mailbox)



@pytest.fixture
def subless_god_account(user_data, firefox_driver):
    id, cookie = login_as_god_user(firefox_driver)
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