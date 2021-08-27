import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait

from PageObjectModels.StripeSignupPage import StripeSignupPage
from .BasePage import BasePage
import time

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class PlanSelectionPage(BasePage):

    @property
    def plan_selection_button(self):
        return self.driver.find_element_by_id(PlanSelectionLocators.plan_select_id)

    def select_plan(self):  # there's only one tier now, will need to overhaul this a lot in the future
        logger.info(f'Selecting plan')
        self.plan_selection_button.click()
        return StripeSignupPage(self.driver)


class PlanSelectionLocators:
    plan_select_id = 'basic-plan-btn'
