import logging

import os

import time
from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage
from PageObjectModels.DashboardPage import DashboardPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage


logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class LoginPage(BasePage):
    @property
    def google_login_button(self):
        return self.driver.find_elements_by_name(LoginLocators.google_login_name)[1]

    @property
    def email_textbox(self):
        return self.driver.find_elements_by_id(LoginLocators.email_textbox_id)[1]

    @property
    def password_textbox(self):
        return self.driver.find_elements_by_id(LoginLocators.pass_textbox_id)[1]

    @property
    def sign_in_button(self):
        return self.driver.find_elements_by_name(LoginLocators.sign_in_button_name)[1]

    @property
    def forgot_password_link(self):
        return self.driver.find_elements_by_xpath(LoginLocators.forgot_pass_xpath)[1]

    @property
    def signup_link(self):
        return self.driver.find_elements_by_xpath(LoginLocators.signup_link_xpath)[0]

    def open(self):
        self.driver.get(f'https://{os.environ["environment"]}.subless.com')
        time.sleep(1)
        return self

    def click_sign_up(self):
        logger.info(f'clicking sign up link')
        from PageObjectModels.SignupPage import SignupPage
        self.signup_link.click()
        return SignupPage(self.driver)

    def click_forgot_pass(self):
        logger.info(f'clicking forgot password')
        self.forgot_password_link.click()

    def sign_in_with_google(self):
        logger.info(f'clicking sign in')
        self.google_login_button.click()

    def sign_in(self, username, password):
        logger.info(f'attempting to sign in')
        logger.info(f'sending {username} to un field')
        self.email_textbox.send_keys(username)

        logger.info(f'sending password')
        self.password_textbox.send_keys(password)
        self.sign_in_button.click()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' not in self.driver.current_url)

        # new user
        if 'register-payment' in self.driver.current_url:
            return PlanSelectionPage(self.driver)

        # returning user
        elif 'user-profile' in self.driver.current_url:
            return DashboardPage(self.driver)

        else:
            raise Exception('Unable to detect redirect page post-login')


class LoginLocators:
    google_login_name = 'googleSignUpButton'
    email_textbox_id = 'signInFormUsername'
    pass_textbox_id = 'signInFormPassword'
    sign_in_button_name = 'signInSubmitButton'
    forgot_pass_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/div[3]/a'
    signup_link_xpath = '/html/body/div[1]/div/div[2]/div[2]/div[2]/div[2]/div[2]/div/form/div[3]/p/a'
    # signup_link_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/div[3]/p/a'