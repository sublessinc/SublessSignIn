import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from .BasePage import BasePage
from .PlanSelectionPage import PlanSelectionPage
import time

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class DashboardLocators:
    manage_billing_button_xpath = '//*[@id="bodyWrapper"]/div[2]/div/div[2]/button'
    user_profile_button_id = 'user'


class DashboardPage(BasePage):
    def go_to_manage_billing(self):
        pass

    def view_user_profile(self):
        pass


