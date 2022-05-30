import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

from PageObjectModels.LoginPage import LoginPage
from PageObjectModels.NavbarPage import NavbarPage
logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class AccountSettingsPage(NavbarPage):
    @property
    def cancel_subscription_button(self):
        return self.driver.find_element_by_id(AccountSettingsLocators.cancel_subscription_id)

    @property
    def confirm_button(self):
        return self.driver.find_element_by_css_selector(AccountSettingsLocators.confirm_button_selector)

    def cancel_subscription(self):
        logger.info(f'Cancelling sub')
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.ID, AccountSettingsLocators.cancel_subscription_id)))
        logger.info(self.__class__.__name__)
        self.cancel_subscription_button.click()
        WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.CSS_SELECTOR, AccountSettingsLocators.confirm_button_selector)))
        self.confirm_button.click()
        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'register-payment' in driver.current_url)
        return LoginPage(self.driver)

class AccountSettingsLocators:
    cancel_subscription_id = "cancel-sub"
    confirm_button_selector = "#mat-dialog-0 > app-warn-dialog > mat-dialog-actions > button:nth-child(2)"