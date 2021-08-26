import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from .BasePage import BasePage
import time

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class PlanSelectionLocators:
    plan_select_id = 'basic-plan-btn'


class PlanSelectionPage(BasePage):
    def __init__(self, driver):
        super().__init__(driver)

    def select_plan(self):  # there's only one tier now, will need to overhaul this a lot in the future
        logger.info(f'Selecting plan')
        el = self.driver.find_element_by_id(PlanSelectionLocators.plan_select_id)
        el.click()
