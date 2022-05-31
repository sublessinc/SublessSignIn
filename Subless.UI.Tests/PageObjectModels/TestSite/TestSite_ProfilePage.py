import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

from PageObjectModels.LoginPage import LoginPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

class TestSite_ProfilePage(object):

    @property
    def activate_link(self):
        return self.driver.find_element_by_css_selector(TestSite_ProfilePage_Locators.profile_selector)

    def click_profile(self):
        logger.info(f'Navigating to profile page')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.CSS_SELECTOR, TestSite_ProfilePage_Locators.activate_selector)))
        logger.info(self.__class__.__name__)
        self.activate_link.click()
        return LoginPage(self.driver)

    def __init__(self, driver):
        self.driver = driver


class TestSite_ProfilePage_Locators:
    activate_selector = "body > a"
