import logging

import os

import time
from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage
from PageObjectModels.DashboardPage import DashboardPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage
from PageObjectModels.TermsPage import TermsPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class OTPConfirmationPage(BasePage):
    @property
    def submit_button(self):
        return self.driver.find_elements_by_xpath(OTPLocators.submit_button_xpath)[0]

    @property
    def otp_field(self):
        return self.driver.find_elements_by_id(OTPLocators.code_textbox_id)[0]

    def confirm_otp(self, otp):
        self.otp_field.send_keys(otp)
        self.submit_button.click()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'confirm' not in self.driver.current_url)
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' not in self.driver.current_url)

        return TermsPage(self.driver)


class OTPLocators:
    code_textbox_id = 'verification_code'
    submit_button_xpath = '//*[@id="confirm"]/div[2]/button'
