import logging

from PageObjectModels.PatronDashboardPage import PatronDashboardPage
from PageObjectModels.TestSite.TestSite_HomePage import TestSite_HomePage

import time
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

def test_no_user_login_button_on_test_site(web_driver, params):
    # WHEN: I visit a partner
    test_site = TestSite_HomePage(web_driver)
    test_site.open()
    logging.info("Waiting test site to load, and login check to run")
    time.sleep(3)
    # THEN: I should be logged in
    assert test_site.login_button.is_displayed()
    assert not test_site.logout_button.is_displayed()


def test_paying_user_logged_in_to_test_site(web_driver, paying_user, params):
    # WHEN: I visit a partner
    test_site = TestSite_HomePage(web_driver)
    test_site.open()
    logging.info("Waiting test site to load, and login check to run")
    time.sleep(3)

    # THEN: I should be logged in
    assert test_site.logout_button.is_displayed()
    assert not test_site.login_button.is_displayed()



def test_paying_user_can_logout_of_test_site(web_driver, paying_user, params):
    # WHEN: I log out of a partner
    test_site = TestSite_HomePage(web_driver)
    test_site.open()
    handles = web_driver.window_handles
    test_site.click_logout()
    post_logout_handles = web_driver.window_handles
    assert len(post_logout_handles) - len(handles) == 1 #should have opened a new tab
    web_driver.switch_to.window(post_logout_handles[1])
    # THEN: I should see a login page
    time.sleep(1)
    assert "login" in web_driver.current_url

    web_driver.close()
    web_driver.switch_to.window(handles[0])

def test_paying_user_hit_pushed(web_driver, subless_activated_creator_user, paying_user, params):
    # WHEN: I visit creator
    dashboard = PatronDashboardPage(web_driver)
    hits_before = dashboard.get_hit_count()
    homepage = TestSite_HomePage(web_driver)
    homepage.open()
    logging.info("Waiting test site to load, and login check to run")
    time.sleep(3)
    homepage.click_profile()
    logging.info("Waiting test site to load, and login check to run")
    time.sleep(3)
    # THEN my hit count should go up by 1
    dashboard = dashboard.open()
    hits_after = dashboard.get_hit_count()
    assert int(hits_after)-int(hits_before) == 1
