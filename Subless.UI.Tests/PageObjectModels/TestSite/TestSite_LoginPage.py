import logging
import os

from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

from PageObjectModels.LoginPage import LoginPage
from PageObjectModels.TestSite.TestSite_LegacyContentPage import TestSite_UriContentPage
from PageObjectModels.TestSite.TestSite_ProfilePage import TestSite_ProfilePage
from PageObjectModels.TestSite.TestSite_TagContentPage import TestSite_TagContentPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class TestSiteLoginPage(object):
    @property
    def login_button(self):
        return self.driver.find_element_by_id(TestSite_HomePage_Locators.login_id)

    @property
    def logout_button(self):
        return self.driver.find_element_by_id(TestSite_HomePage_Locators.logout_id)

    @property
    def profile_link(self):
        return self.driver.find_element_by_css_selector(TestSite_HomePage_Locators.profile_selector)

    @property
    def tag_content_link(self):
        return self.driver.find_element_by_css_selector(TestSite_HomePage_Locators.tag_content_selector)

    @property
    def uri_content_link(self):
        return self.driver.find_element_by_css_selector(TestSite_HomePage_Locators.uri_content_selector)

    def open(self):
        self.driver.get(f'https://{Keys.test_client_uri}')
        return self

    def click_login(self):
        logger.info(f'Navigating to subless login')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.ID, TestSite_HomePage_Locators.login_id)))
        logger.info(self.__class__.__name__)
        self.login_button.click()
        return LoginPage(self.driver)

    def click_logout(self):
        logger.info(f'Logging out')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.ID, TestSite_HomePage_Locators.logout_id)))
        logger.info(self.__class__.__name__)
        self.logout_button.click()
        return LoginPage(self.driver)

    def click_profile(self):
        logger.info(f'Navigating to profile page')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.CSS_SELECTOR, TestSite_HomePage_Locators.profile_selector)))
        logger.info(self.__class__.__name__)
        self.profile_link.click()
        return TestSite_ProfilePage(self.driver)

    def click_uri_content(self):
        logger.info(f'Navigating to profile page')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.CSS_SELECTOR, TestSite_HomePage_Locators.uri_content_selector)))
        logger.info(self.__class__.__name__)
        self.uri_content_link.click()
        return TestSite_UriContentPage(self.driver)

    def click_tag_content(self):
        logger.info(f'Navigating to profile page')
        WebDriverWait(self.driver, 10).until(lambda driver: 'pythonclient' in driver.current_url)
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.CSS_SELECTOR, TestSite_HomePage_Locators.uri_content_selector)))
        logger.info(self.__class__.__name__)
        self.tag_content_link.click()
        return TestSite_TagContentPage(self.driver)

    def __init__(self, driver):
        self.driver = driver


class TestSite_HomePage_Locators:
    login_id = "btnLogin"
    logout_id = "btnLogout"
    profile_selector = "body > a:nth-child(5)"
    tag_content_selector = "body > a:nth-child(7)"
    uri_content_selector = "body > a:nth-child(9)"
