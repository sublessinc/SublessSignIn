import logging

from selenium.webdriver.support.ui import WebDriverWait
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class BasePage(object):
    @property
    def logout_button(self):
        return self.driver.find_element_by_css_selector(BasePageLocators.logout_button_id)

    def __init__(self, driver):
        self.driver = driver

    def logout(self):
        logger.info(f'Logging Out')
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.ID, BasePageLocators.logout_button_id)))
        logger.info(self.__class__.__name__)
        self.logout_button.click()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' in driver.current_url)


class BasePageLocators:
    logout_button_id = 'logout'