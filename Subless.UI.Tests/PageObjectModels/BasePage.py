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
        return self.driver.find_element_by_css_selector(BasePageLocators.logout_button_selector)

    def __init__(self, driver):
        self.driver = driver
        self.wait_for_load()

    def logout(self):
        logger.info(f'Logging Out')
        self.wait_for_load()
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.CSS_SELECTOR, BasePageLocators.logout_button_selector)))
        WebDriverWait(self.driver, 10).until(EC.element_to_be_clickable((By.CSS_SELECTOR, BasePageLocators.logout_button_selector)))

        logger.info(self.__class__.__name__)
        self.logout_button.click()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' in driver.current_url)

    def wait_for_load(self):
        WebDriverWait(self.driver, 10).until(EC.invisibility_of_element_located((By.ID, BasePageLocators.loading_id)));

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
    logout_button_selector = '#logout, #logout2'
    loading_id = "loadingContainer"
