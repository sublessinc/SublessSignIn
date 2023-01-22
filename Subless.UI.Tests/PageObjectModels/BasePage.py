import logging
import time

from selenium.common.exceptions import TimeoutException
from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

class BasePage(object):

    @property
    def minimal_nav_logout_button(self):
        return self.driver.find_element_by_id(BasePageLocators.minimal_nav_logout_button_id)

    @property
    def logout_button(self):
        return self.driver.find_element_by_id(BasePageLocators.logout_button_id)

    @property
    def account_settings_button(self):
        return self.driver.find_element_by_css_selector(BasePageLocators.account_settings_selector)

    def __init__(self, driver):
        self.driver = driver
        self.wait_for_load()

    def logout(self):
        logger.info(f'Logging Out')
        logger.info(self.__class__.__name__)

        self.wait_for_load()

        if check_exists_by_id(BasePageLocators.logout_button_id, self.driver):
            WebDriverWait(self.driver, 10).until(
                EC.element_to_be_clickable((By.CSS_SELECTOR, BasePageLocators.logout_button_selector)))
            logger.info("clicking logout")
            self.logout_button.click()

        if check_exists_by_id(BasePageLocators.minimal_nav_logout_button_id, self.driver):
            WebDriverWait(self.driver, 10).until(
                EC.element_to_be_clickable((By.CSS_SELECTOR, BasePageLocators.minimal_nav_logout_button_selector)))
            logger.info("clicking logout2")
            self.minimal_nav_logout_button.click()

        logger.info("waiting for redirect to login page")
        # wait for redirect
        logger.info(self.driver.current_url)
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' in driver.current_url)

    def wait_for_load(self):
        if check_exists_by_id(BasePageLocators.loading_id, self.driver):
            WebDriverWait(self.driver, 10).until(EC.invisibility_of_element_located((By.ID, BasePageLocators.loading_id)))

    def navigate_to_account_settings(self):
        from .AccountSettingsPage import AccountSettingsPage
        logger.info(f'Navigating to change plan')
        time.sleep(1)
        WebDriverWait(self.driver, 10).until(lambda driver: 'subless' in driver.current_url)
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.CSS_SELECTOR, BasePageLocators.account_settings_selector)))
        WebDriverWait(self.driver, 10).until(EC.element_to_be_clickable((By.CSS_SELECTOR, BasePageLocators.account_settings_selector)))
        logger.info(self.__class__.__name__)
        self.account_settings_button.click()
        self.accept_alert_if_present()
        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'account-settings' in driver.current_url)
        return AccountSettingsPage(self.driver)

    def accept_alert_if_present(self):
        try:
            WebDriverWait(self.driver, 3).until(EC.alert_is_present(),
                                           'Timed out waiting alert to appear.')

            alert = self.driver.switch_to.alert
            alert.accept()
            print("alert accepted")
        except TimeoutException:
            print("no alert")
def check_exists_by_id(id, driver):
    from selenium.common.exceptions import NoSuchElementException
    try:
        driver.find_element_by_id(id)
    except NoSuchElementException:
        return False
    return True

def check_exists_by_xpath(xpath, driver):
    from selenium.common.exceptions import NoSuchElementException
    try:
        driver.find_element_by_xpath(xpath)
    except NoSuchElementException:
        return False
    return True



class BasePageLocators:
    minimal_nav_logout_button_selector = '#logout2'
    minimal_nav_logout_button_id = 'logout2'
    logout_button_selector = '#logout'
    logout_button_id = 'logout'
    loading_id = "loadingContainer"
    account_settings_selector = "#account, #account-settings2"