import time

import pytest

from Fixtures.drivers import chrome_driver
from PageObjectModels.LoginPage import LoginPage
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


@pytest.mark.skip("not yet implemented")
def test_admin_dashboard(chrome_driver):
    # GIVEN: I am on the Subless login page, as an admin user

    # WHEN: I enter my username and password and click 'sign in'

    # THEN: I should be taken to my dashboard

    # AND: I should have all available admin controls
    pass


@pytest.mark.skip("not yet implemented")
def test_partner_dashboard(chrome_driver):
    # GIVEN: I am on the Subless login page, as an partner user

    # WHEN: I enter my username and password and click 'sign in'

    # THEN: I should be taken to my dashboard

    # AND: I should have all available partner controls
    pass


@pytest.mark.skip("not yet implemented")
def test_creator_dashboard(chrome_driver):
    # GIVEN: I am on the Subless login page, as an partner user

    # WHEN: I enter my username and password and click 'sign in'

    # THEN: I should be taken to my dashboard

    # AND: I should have all available creator controls
    pass


@pytest.mark.skip("not yet implemented")
def test_consumer_dashboard(chrome_driver):
    # GIVEN: I am on the Subless login page, as an admin user

    # WHEN: I enter my username and password and click 'sign in'

    # THEN: I should be taken to my dashboard

    # AND: I should only have minimal controls
    pass
