import logging
import time

from selenium.webdriver.support import expected_conditions as EC

from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait

from .BasePage import BasePage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class NavbarPage(BasePage):
    @property
    def change_plan_button(self):
        return self.driver.find_element_by_id(NavbarPageLocators.change_plan_id)

    @property
    def billing_button(self):
        return self.driver.find_element_by_id(NavbarPageLocators.billing_id)

    @property
    def cancellation_message(self):
        # switch to ID before merge
        return self.driver.find_element_by_xpath(NavbarPageLocators.cancellation_message_xpath)

    @property
    def resubscribe_button(self):
        # switch to ID before merge
        return self.driver.find_element_by_xpath(NavbarPageLocators.resubscribe_button_xpath)
    def __init__(self, driver):
        self.driver = driver

    def navigate_to_change_plan(self):
        from .PlanSelectionPage import PlanSelectionPage
        logger.info(f'Navigating to change plan')
        WebDriverWait(self.driver, 10).until(lambda driver: 'subless' in driver.current_url)
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.ID, NavbarPageLocators.change_plan_id)))
        logger.info(self.__class__.__name__)
        self.change_plan_button.click()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'plan' in driver.current_url)
        return PlanSelectionPage(self.driver)

    def navigate_to_billing(self):
        from .ExternalPages.StripeManagePage import StripeManagementPage
        logger.info(f'Navigating to change plan')
        WebDriverWait(self.driver, 10).until(lambda driver: 'subless' in driver.current_url)
        WebDriverWait(self.driver, 10).until(EC.presence_of_element_located((By.ID, NavbarPageLocators.billing_id)))
        logger.info(self.__class__.__name__)
        self.billing_button.click()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'plan' in self.driver.current_url)
        return StripeManagementPage(self.driver)



class NavbarPageLocators:
    partner_profile_id = "partner"
    patron_profile_id = "user"
    billing_id = "billing"
    payout_id = "payout"
    change_plan_id = "plan"
    integration_settings_id = "integration"
    cancellation_message_id = "cancellation-warning-message"
    cancellation_message_xpath = "/html/body/app-root/app-nav/mat-sidenav-container/mat-sidenav-content/app-cancellation-warning/mat-card/mat-card-content/b"
    resubscribe_button_id = "resubscribe"
    resubscribe_button_xpath = "/html/body/app-root/app-nav/mat-sidenav-container/mat-sidenav-content/app-cancellation-warning/mat-card/button"

