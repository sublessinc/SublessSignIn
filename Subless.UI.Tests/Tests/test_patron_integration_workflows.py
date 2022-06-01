
from PageObjectModels.DashboardPage import DashboardPage
from PageObjectModels.TestSite.TestSite_HomePage import TestSite_HomePage

import time

def test_no_user_login_button_on_test_site(web_driver, params):
    # WHEN: I visit a partner
    test_site = TestSite_HomePage(web_driver)
    test_site.open()
    time.sleep(3)
    # THEN: I should be logged in
    assert test_site.login_button.is_displayed()
    assert not test_site.logout_button.is_displayed()


def test_paying_user_logged_in_to_test_site(web_driver, paying_user, params):
    # WHEN: I visit a partner
    test_site = TestSite_HomePage(web_driver)
    test_site.open()
    time.sleep(3)

    # THEN: I should be logged in
    assert test_site.logout_button.is_displayed()
    assert not test_site.login_button.is_displayed()



def test_paying_user_can_logout_of_test_site(web_driver, paying_user, params):
    # WHEN: I log out of a partner
    test_site = TestSite_HomePage(web_driver)
    test_site.open()
    test_site.click_logout()
    time.sleep(3)

    # THEN: I should see a login page
    assert "login" in web_driver.current_url

def test_paying_user_hit_pushed(web_driver, subless_activated_creator_user, paying_user, params):
    # WHEN: I visit creator
    dashboard = DashboardPage(web_driver)
    hits_before = dashboard.get_hit_count()
    homepage = TestSite_HomePage(web_driver)
    homepage.open()
    time.sleep(3)
    homepage.click_profile()
    time.sleep(3)
    # THEN my hit count should go up by 1
    dashboard = dashboard.open()
    hits_after = dashboard.get_hit_count()
    assert int(hits_after)-int(hits_before) == 1
