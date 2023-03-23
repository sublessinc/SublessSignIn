import logging

import time

from PageObjectModels.ExternalPages.StripeSignupPage import StripeSignupPage
from Keys.Keys import Keys
from .BasePage import BasePage
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By

from .NavbarPage import NavbarPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class PlanSelectionPage(NavbarPage):

    @property
    def plan_5_selection_button(self):
        return self.driver.find_element_by_id(PlanSelectionLocators.plan_5_button_id)

    @property
    def plan_10_selection_button(self):
        return self.driver.find_element_by_id(PlanSelectionLocators.plan_10_button_id)

    @property
    def is_5_plan_selected(self):
        plan_button = self.driver.find_element_by_id(PlanSelectionLocators.plan_5_button_id)
        return plan_button.get_attribute("aria-pressed")

    @property
    def is_10_plan_selected(self):
        plan_button = self.driver.find_element_by_id(PlanSelectionLocators.plan_10_button_id)
        return plan_button.get_attribute("aria-pressed")

    @property
    def plan_confirm_button(self):
        return self.driver.find_element_by_xpath(PlanSelectionLocators.plan_confirm_xpath)

    def open(self):
        self.driver.get(f'https://{Keys.subless_uri}/register-payment')
        logging.info("Waiting for register payment page load to complete")
        time.sleep(3)
        return self

    def select_plan_5(self):  # there's only one tier now, will need to overhaul this a lot in the future
        logger.info(f'Selecting plan')
        self.plan_5_selection_button.click()
        self.plan_confirm_button.click()
        logging.info("Waiting for plan change to execute")
        time.sleep(3)
        return StripeSignupPage(self.driver)

    def change_plan_10(self):  # there's only one tier now, will need to overhaul this a lot in the future
        logger.info(f'Changing Plan')
        self.plan_10_selection_button.click()
        self.plan_confirm_button.click()
        logging.info("Waiting for plan change to execute")
        time.sleep(3)
        return StripeSignupPage(self.driver)

class PlanSelectionLocators:
    plan_confirm_xpath = '/html/body/app-root/app-nav/mat-sidenav-container/mat-sidenav-content/app-register-payment/div/mat-card/button'
    plan_5_button_id = 'mat-button-toggle-1-button'
    plan_10_button_id = 'mat-button-toggle-2-button'
    plan_15_button_id = 'mat-button-toggle-3-button'
