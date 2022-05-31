import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

from PageObjectModels.LoginPage import LoginPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

class TestSite_HomePage(object):
    @property
    def login_button(self):
        return self.driver.find_element_by_id(TestSite_HomePage_Locators.login_id)

    @property
    def logout_button(self):
        return self.driver.find_element_by_id(TestSite_HomePage_Locators.logout_id)

    @property
    def profile_link(self):
        return self.driver.find_element_by_css_selector(TestSite_HomePage_Locators.profile_selector)

    def click_login(self):
        logger.info(f'Navigating to subless login')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.ID, TestSite_HomePage_Locators.login_id)))
        logger.info(self.__class__.__name__)
        self.login_button.click()
        return LoginPage(self.driver)

    def click_logout(self):
        logger.info(f'Logging out')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.ID, TestSite_HomePage_Locators.logout_id)))
        logger.info(self.__class__.__name__)
        self.logout_button.click()
        return LoginPage(self.driver)

    def click_profile(self):
        logger.info(f'Navigating to profile page')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.CSS_SELECTOR, TestSite_HomePage_Locators.profile_selector)))
        logger.info(self.__class__.__name__)
        self.profile_link.click()
        return LoginPage(self.driver)

    def __init__(self, driver):
        self.driver = driver

class TestSite_HomePage_Locators:
    login_id = "btnLogin"
    logout_id = "btnLogout"
    profile_selector = "body > a"