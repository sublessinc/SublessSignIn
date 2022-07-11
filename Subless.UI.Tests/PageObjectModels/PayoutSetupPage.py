import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class PayoutSetupPage(BasePage):

    @property
    def creator_paypal_textbox(self):
        return self.driver.find_element_by_css_selector(PayoutSetupPage_Locators.creator_paypal_selector)

    @property
    def creator_paypal_submit(self):
        return self.driver.find_element_by_css_selector(PayoutSetupPage_Locators.creator_submit_selector)

    def enter_creator_paypal(self, paypalid):
        logger.info(f'entering paypal id')
        WebDriverWait(self.driver, 10).until(
            EC.visibility_of_element_located((By.CSS_SELECTOR, PayoutSetupPage_Locators.creator_paypal_selector)))
        self.creator_paypal_textbox.send_keys(paypalid)

    def submit_creator_paypal(self):
        logger.info(f'submitting paypal id')
        WebDriverWait(self.driver, 10).until(
            EC.element_to_be_clickable((By.CSS_SELECTOR, PayoutSetupPage_Locators.creator_submit_selector)))
        self.creator_paypal_submit.click()
        WebDriverWait(self.driver, 10).until(lambda driver: 'payout' not in driver.current_url)


class PayoutSetupPage_Locators:
    creator_paypal_selector = '#mat-input-0'
    creator_submit_selector = 'button.mat-focus-indicator'
