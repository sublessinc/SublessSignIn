import logging

from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage
from PageObjectModels.DashboardPage import DashboardPage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class SignupPage(BasePage):
    @property
    def google_login_button(self):
        return self.driver.find_elements_by_name(SignupLocators.google_login_name)[1]

    @property
    def email_textbox(self):
        return self.driver.find_elements_by_id(SignupLocators.email_textbox_id)[1]

    @property
    def password_textbox(self):
        return self.driver.find_elements_by_id(SignupLocators.pass_textbox_id)[1]

    @property
    def sign_up_button(self):
        return self.driver.find_elements_by_name(SignupLocators.sign_up_button_name)[1]

    @property
    def sign_in_link(self):
        return self.driver.find_elements_by_xpath(SignupLocators.sign_in_link_xpath)[1]

    def sign_up(self, un, password):
        logger.info(f'attempting to sign in')
        self.email_textbox.sendkeys(un)
        self.password_textbox.sendkeys(password)
        self.sign_up_button.click()

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


class SignupLocators:
    sign_in_link_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[2]/div/form/p/div/a'
    sign_up_button_name = 'signUpButton'
    google_login_name = 'googleSignUpButton'
    email_textbox_id = 'signInFormUsername'
    pass_textbox_id = 'signInFormPassword'