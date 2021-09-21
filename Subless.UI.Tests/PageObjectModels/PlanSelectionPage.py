import logging

from PageObjectModels.ExternalPages.StripeSignupPage import StripeSignupPage
from .BasePage import BasePage

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
