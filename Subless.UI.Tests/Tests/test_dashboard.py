import time

import pytest

from PageObjectModels.LoginPage import LoginPage
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


@pytest.mark.skip("not yet implemented")
def test_admin_dashboard(chrome_driver, params):
    # GIVEN: I am on the Subless login page, as an admin user
    login_page = LoginPage(chrome_driver).open()

    # WHEN: I enter my username and password and click 'sign in'
    dashboard_page = login_page.sign_in('sublesstestuser+admin', params['password'])

    # THEN: I should be taken to my dashboard

    # AND: I should have all available admin controls
    pass


@pytest.mark.skip("not yet implemented")
def test_partner_dashboard(chrome_driver, params):
    # GIVEN: I am on the Subless login page, as an partner user
    login_page = LoginPage(chrome_driver).open()

    # WHEN: I enter my username and password and click 'sign in'
    dashboard_page = login_page.sign_in('sublesstestuser+partner', params['password'])

    # THEN: I should be taken to my dashboard

    # AND: I should have all available partner controls
    pass


@pytest.mark.skip("not yet implemented")
def test_creator_dashboard(chrome_driver, params):
    # GIVEN: I am on the Subless login page, as an partner user
    login_page = LoginPage(chrome_driver).open()

    # WHEN: I enter my username and password and click 'sign in'
    dashboard_page = login_page.sign_in('sublesstestuser+creator', params['password'])

    # THEN: I should be taken to my dashboard

    # AND: I should have all available creator controls
    pass


@pytest.mark.skip("not yet implemented")
def test_consumer_dashboard(chrome_driver, params):
    # GIVEN: I am on the Subless login page, as an admin user
    login_page = LoginPage(chrome_driver).open()

    # WHEN: I enter my username and password and click 'sign in'
    dashboard_page = login_page.sign_in('sublesstestuser+default', params['password'])

    # THEN: I should be taken to my dashboard
    # AND: I should only have minimal controls

    pass
