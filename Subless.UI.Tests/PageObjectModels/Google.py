
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from .BasePage import BasePage
import time
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()

class Google(BasePage):
    search_xpath = '/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input'

    search_field = (By.XPATH, search_xpath)

    def open(self):
        self.driver.get('https://www.google.com')
        logger.info('opened google I guess')
        return self

    def enter_search(self, search_terms):
        logger.info('searching')
        search_xpath = '/html/body/div[1]/div[3]/form/div[1]/div[1]/div[1]/div/div[2]/input'
        el = self.driver.find_element((By.XPATH, search_xpath))
        logger.info('dying')
        el.click()
        el.send_keys(search_terms)
