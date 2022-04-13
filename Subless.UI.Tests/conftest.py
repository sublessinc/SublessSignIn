import os

import pytest
from selenium import webdriver
from webdriver_manager.chrome import ChromeDriverManager
from webdriver_manager.firefox import GeckoDriverManager

from ApiLib import Admin
from EmailLib.MailSlurp import get_inbox_from_name
from PageObjectModels.LoginPage import LoginPage
from UsersLib.Users import get_user_id_and_cookie

print(os.getcwd())

def pytest_addoption(parser):
    parser.addoption("--password", action="store", help="override login password for all tests")
    parser.addoption("--environment", action="store", default='dev', help="dev/test/pay")


@pytest.fixture(autouse=True)
def params(request):
    params = {'password': request.config.getoption('--password') \
        if request.config.getoption('--password') is not None \
        else 'SublessTestUser'}
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
    yield 'null' # todo:  temporarily disabling in the laziest way possible
    # from EmailLib import MailSlurp
    #
    # # create
    # inbox = MailSlurp.create_inbox()
    #
    # yield inbox
    #
    # # delete
    # MailSlurp.delete_inbox_by_id(inbox.id)


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

    mailbox = get_inbox_from_name('BasicUser')
    attempt_to_delete_user(firefox_driver, mailbox)

    # create
    id, cookie = create_user(firefox_driver, mailbox)
    yield id, cookie

    # LoginPage(firefox_driver).logout()  # do we need to logout here??

    # HACK: delete user
    # I hate this.
    attempt_to_delete_user(firefox_driver, mailbox)


def attempt_to_delete_user(firefox_driver, mailbox):
    from ApiLib import User
    try:
        LoginPage(firefox_driver).open().sign_in(mailbox.email_address, "SublessTestUser")
        id, cookie = get_user_id_and_cookie(firefox_driver)
        User.delete(cookie)
    except:  # awful.
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

    mailbox = get_inbox_from_name('AdminUser')
    # create
    id, cookie = create_user(firefox_driver, mailbox)
    Admin.set_admin(id, god_cookie)
    # LoginPage(firefox_driver).logout()  # do we need to logout here??

    yield id, cookie

    User.delete(cookie)


@pytest.fixture
def subless_partner_account():
    from ApiLib import User
    from UsersLib.Users import create_user

    mailbox = get_inbox_from_name('PartnerUser')
    # create
    id, cookie = create_user(firefox_driver, mailbox)
    # LoginPage(firefox_driver).logout()  # do we need to logout here??
    # TODO:  set partner perms

    yield id, cookie

    User.delete(cookie)


@pytest.fixture
def subless_creator_user():
    from ApiLib import User
    from UsersLib.Users import create_user

    mailbox = get_inbox_from_name('CreatorUser')
    # create
    id, cookie = create_user(firefox_driver, mailbox)
    # TODO: set creator perms
    # Admin.set_admin(id, test_data['GodUser']['token'])
    # LoginPage(firefox_driver).logout() # # do we need to logout here??

    yield id, cookie

    User.delete(cookie)


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