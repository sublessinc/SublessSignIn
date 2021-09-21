import os

import pytest
from selenium import webdriver
from webdriver_manager.chrome import ChromeDriverManager
from webdriver_manager.firefox import GeckoDriverManager

from EmailLib import MailSlurp
from UsersLib.Users import create_user


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


@pytest.fixture
def mailslurp_inbox():
    # create
    inbox = MailSlurp.create_inbox()

    yield inbox

    # delete
    MailSlurp.delete_inbox_by_id(inbox.id)


# ideally this would be some sort of API call
#   but we're working with what we've got available
@pytest.fixture
def subless_account(mailslurp_inbox, chrome_driver):
    # create
    id, token = create_user(chrome_driver, mailslurp_inbox)

    yield id

    # todo: do not have this functionality yet
    # destroy


# todo: stubbing this for now as it may be useful later
@pytest.fixture
def subless_admin_account(subless_account):
    # update perms

    yield 'baz'


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