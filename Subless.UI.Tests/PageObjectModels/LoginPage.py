import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from .BasePage import BasePage
import time

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class LoginLocators:
    google_login_name = 'googleSignUpButton'
    email_textbox_id = 'signInFormUsername'
    pass_textbox_id = 'signInFormPassword'
    sign_in_button_name = 'signInSubmitButton'
    forgot_pass_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/div[3]/a'
    signup_link_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/div[4]/p/div/a'


class LoginPage(BasePage):
    def open(self):
        self.driver.get('https://pay.subless.com')
        return self

    # there's two of all of these elements for some reason, you need the second one
    def enter_un(self, username):
        logger.info(f'sending {username} to un field')
        el = self.driver.find_elements_by_id(LoginLocators.email_textbox_id)[1]
        el.send_keys(username)

    def enter_pass(self, password):
        logger.info(f'sending password')
        el = self.driver.find_elements_by_id(LoginLocators.pass_textbox_id)[1]
        el.send_keys(password)

    def click_sign_up(self):
        logger.info(f'clicking sign up button')
        el = self.driver.find_elements_by_xpath(LoginLocators.signup_link_xpath)[1]
        el.click()

    def click_forgot_pass(self):
        logger.info(f'clicking forgot password')
        el = self.driver.find_elements_by_xpath(LoginLocators.forgot_pass_xpath)[1]
        el.click()

    def click_google_signin(self):
        logger.info(f'clicking sign in')
        el = self.driver.find_elements_by_name(LoginLocators.google_login_name)[1]
        el.click()

    def click_sign_in(self):
        logger.info(f'clicking sign in')
        el = self.driver.find_elements_by_name(LoginLocators.sign_in_button_name)[1]
        el.click()

    def sign_in(self, un, password):
        logger.info(f'attempting to sign in')
        self.enter_un(un)
        self.enter_pass(password)
        self.click_sign_in()
        # self.driver.wait
        header_image_xpath = '/html/body/app-root/app-register-payment/div[2]/app-nav/mat-toolbar/span[1]/div'
        header_image = WebDriverWait(self.driver, 10).until(
            EC.presence_of_element_located((By.XPATH, header_image_xpath)))
