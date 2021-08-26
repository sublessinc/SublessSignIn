

import logging

from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage
from PageObjectModels.DashboardPage import DashboardPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class SignupLocators():
    sign_in_link_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[2]/div/form/p/div/a'
    sign_up_button_name = 'signUpButton'
    google_login_name = 'googleSignUpButton'
    email_textbox_id = 'signInFormUsername'
    pass_textbox_id = 'signInFormPassword'


# the signup page is near-identical to the sign in page, so just inherit it
class SignupPage(BasePage):
    def open(self):
        self.driver.get('https://dev.subless.com')  # todo: this needs to be pulled into env vars / configs
        return self

    # there's two of all of these elements for some reason, you need the second one
    def enter_un(self, username):
        logger.info(f'sending {username} to un field')
        el = self.driver.find_elements_by_id(SignupLocators.email_textbox_id)[1]
        el.send_keys(username)

    def enter_pass(self, password):
        logger.info(f'sending password')
        el = self.driver.find_elements_by_id(SignupLocators.pass_textbox_id)[1]
        el.send_keys(password)

    def click_sign_in_link(self):
        logger.info(f'clicking sign up button')
        from PageObjectModels.LoginPage import LoginPage
        el = self.driver.find_elements_by_xpath(SignupLocators.sign_in_link_xpath)[1]
        el.click()
        return LoginPage(self.driver)

    def click_google_signin(self):
        logger.info(f'clicking sign in')
        el = self.driver.find_elements_by_name(SignupLocators.google_login_name)[1]
        el.click()

    def click_sign_up_button(self):
        logger.info(f'clicking sign in')
        el = self.driver.find_elements_by_name(SignupLocators.sign_up_button_name)[1]
        el.click()

    def sign_up(self, un, password):
        logger.info(f'attempting to sign in')
        self.enter_un(un)
        self.enter_pass(password)
        self.click_sign_up_button()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'signup' not in driver.current_url and 'login' not in driver.current_url)

        # new user
        if 'register-payment' in self.driver.current_url:
            return PlanSelectionPage(self.driver)

        # returning user
        elif 'user-profile' in self.driver.current_url:
            return DashboardPage(self.driver)

        else:
            raise Exception('Unable to detect redirect page post-login')
