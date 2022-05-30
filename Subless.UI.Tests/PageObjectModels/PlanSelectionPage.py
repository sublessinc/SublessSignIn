import logging

import time

from PageObjectModels.ExternalPages.StripeSignupPage import StripeSignupPage
from .BasePage import BasePage
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.common.by import By

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class PlanSelectionPage(BasePage):

    @property
    def plan_selection_button(self):
        return self.driver.find_element_by_id(PlanSelectionLocators.plan_select_id)

    @property
    def plan_confirm_button(self):
        return self.driver.find_element_by_xpath(PlanSelectionLocators.plan_confirm_xpath)

    def select_plan(self):  # there's only one tier now, will need to overhaul this a lot in the future
        logger.info(f'Selecting plan')
        self.plan_selection_button.click()
        self.plan_confirm_button.click()
        time.sleep(3)
        return StripeSignupPage(self.driver)

    @property
    def logout_button(self):
        return self.driver.find_element_by_id(PlanSelectionLocators.logout_button_id)

    def logout(self):
        logger.info(f'Logging Out')
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.ID, PlanSelectionLocators.logout_button_id)))
        logger.info(self.__class__.__name__)
        self.logout_button.click()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' in driver.current_url)

class PlanSelectionLocators:
    plan_select_id = 'mat-button-toggle-1-button'
    plan_confirm_xpath = '/html/body/app-root/app-nav/mat-sidenav-container/mat-sidenav-content/app-register-payment/div/mat-card/button'
    logout_button_id = 'logout2'
