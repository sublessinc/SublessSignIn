import logging

from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage
from PageObjectModels.DashboardPage import DashboardPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage


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
        self.driver.get('https://pay.subless.com')  # todo: this needs to be pulled into env vars / configs
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

    def click_sign_up_link(self):
        logger.info(f'clicking sign up link')
        from PageObjectModels.SignupPage import SignupPage
        el = self.driver.find_elements_by_xpath(LoginLocators.signup_link_xpath)[1]
        el.click()
        return SignupPage(self.driver)

    def click_forgot_pass(self):
        logger.info(f'clicking forgot password')
        el = self.driver.find_elements_by_xpath(LoginLocators.forgot_pass_xpath)[1]
        el.click()

    def click_google_signin(self):
        logger.info(f'clicking sign in')
        el = self.driver.find_elements_by_name(LoginLocators.google_login_name)[1]
        el.click()

    def click_sign_in_button(self):
        logger.info(f'clicking sign in')
        el = self.driver.find_elements_by_name(LoginLocators.sign_in_button_name)[1]
        el.click()

    def sign_in(self, un, password):
        logger.info(f'attempting to sign in')
        self.enter_un(un)
        self.enter_pass(password)
        self.click_sign_in_button()

        # wait for redirect
        WebDriverWait(self.driver, 10).until(lambda driver: 'login' not in driver.current_url)

        # new user
        if 'register-payment' in self.driver.current_url:
            return PlanSelectionPage(self.driver)

        # returning user
        elif 'user-profile' in self.driver.current_url:
            return DashboardPage(self.driver)

        else:
            raise Exception('Unable to detect redirect page post-login')