import logging

from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage
from PageObjectModels.OTPConfirmationPage import OTPConfirmationPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class SignupPage(BasePage):
    @property
    def google_login_button(self):
        return self.driver.find_elements_by_name(SignupLocators.google_login_name)[1]

    @property
    def email_textbox(self):
        return self.driver.find_elements_by_xpath(SignupLocators.email_textbox_xpath)[0]

    @property
    def password_textbox(self):
        return self.driver.find_elements_by_xpath(SignupLocators.pass_textbox_xpath)[0]

    @property
    def sign_up_button(self):
        return self.driver.find_elements_by_name(SignupLocators.sign_up_button_name)[0]

    @property
    def sign_in_link(self):
        return self.driver.find_elements_by_xpath(SignupLocators.sign_in_link_xpath)[1]

    def sign_up(self, un, password):
        logger.info(f'attempting to sign in')
        self.email_textbox.send_keys(un)
        self.password_textbox.send_keys(password)
        self.sign_up_button.click()

        return OTPConfirmationPage(self.driver)


class SignupLocators:
    sign_in_link_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[2]/div/form/p/div/a'
    sign_up_button_name = 'signUpButton'
    sign_up_button_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[2]/div/form/button'
    google_login_name = 'googleSignUpButton'
    # email_textbox_xpath = '/html/body/div[1]/div/div[2]/div[2]/div[3]/div[2]/div/form/div[1]/input'
    email_textbox_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[2]/div/form/div[1]/input'
    pass_textbox_id = 'signInFormPassword'
    pass_textbox_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[2]/div/form/input[2]'