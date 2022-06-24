import logging

import os

import time
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC
from PageObjectModels.BasePage import BasePage, BasePageLocators
from PageObjectModels.CreatorDashboardPage import CreatorDashboardPage
from PageObjectModels.PatronDashboardPage import PatronDashboardPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage
from selenium.webdriver.common.by import By

from PageObjectModels.TermsPage import TermsPage

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
        try:
            WebDriverWait(self.driver, 10).until(
                EC.element_to_be_clickable((By.CSS_SELECTOR, LoginLocators.signup_link_selector)))
            link = self.driver.find_element_by_css_selector(LoginLocators.signup_link_selector)
            return link
        except Exception as e:
            raise


    def open(self):
        self.driver.get(f'https://{os.environ["environment"]}.subless.com')
        logging.info("Waiting for login page redirect to complete")
        time.sleep(5)
        # WebDriverWait(self.driver, 10).until(
        #     EC.presence_of_element_located((By.NAME, LoginLocators.sign_in_button_name))
        #     or
        #     EC.presence_of_element_located((By.ID, BasePageLocators.logout_button_id))
        # )
        if ('login' not in self.driver.current_url):
            BasePage(self.driver).logout()
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' in self.driver.current_url)
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

        if 'The username or password you entered is invalid' in self.driver.find_element_by_tag_name('body').text:
            raise Exception('Invalid user, could not login')
        if 'User is not confirmed.' in self.driver.find_element_by_tag_name('body').text:
            raise Exception('User is not confirmed, could not login')

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' not in self.driver.current_url)

        # new user
        if 'register-payment' in self.driver.current_url:
            return PlanSelectionPage(self.driver)

        # returning user
        elif 'user-profile' in self.driver.current_url:
            return PatronDashboardPage(self.driver)

        elif 'creator-profile' in self.driver.current_url:
            return CreatorDashboardPage(self.driver)

        elif 'terms' in self.driver.current_url:
            return TermsPage(self.driver)

        else:
            raise Exception('Unable to detect redirect page post-login')


class LoginLocators:
    google_login_name = 'googleSignUpButton'
    email_textbox_id = 'signInFormUsername'
    pass_textbox_id = 'signInFormPassword'
    sign_in_button_name = 'signInSubmitButton'
    forgot_pass_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/div[3]/a'
    signup_link_selector = 'body > div.container > div > div.modal-content.background-customizable.modal-content' \
                           '-desktop.visible-md.visible-lg > div.modal-body > div:nth-child(2) > ' \
                           'div.panel.panel-left-border.col-md-6.col-lg-6 > div:nth-child(2) > div > form > ' \
                           'div:nth-child(10) > p > a '
    # signup_link_xpath = '/html/body/div[1]/div/div[2]/div[2]/div[2]/div[2]/div[2]/div/form/div[3]/p/a'
    # signup_link_xpath = '/html/body/div[1]/div/div[2]/div[2]/div[2]/div[2]/div/div/form/div[3]/p/a'