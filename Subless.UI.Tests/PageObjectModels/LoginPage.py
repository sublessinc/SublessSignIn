from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.ui import WebDriverWait
from .BasePage import BasePage
import time


class LoginPage(BasePage):
    google_login_name = 'googleSignUpButton'
    email_textbox_id = 'signInFormUsername'
    pass_textbox_id = 'signInFormPassword'
    sign_in_button_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/input[3]'
    forgot_pass_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/div[3]/a'
    signup_link_xpath = '/html/body/div[1]/div/div[1]/div[2]/div[2]/div[3]/div[2]/div/form/div[4]/p/div/a'

    UN_FIELD = (By.ID, email_textbox_id)
    PASS_FIELD = (By.ID, pass_textbox_id)
    GOOGLE_BUTTON = (By.NAME, google_login_name)
    SIGN_IN_BUTTON = (By.XPATH, sign_in_button_xpath)
    FORGOT_PASS_LINK = (By.XPATH, forgot_pass_xpath)
    SIGNUP_LINK = (By.XPATH, signup_link_xpath)

    def open(self):
        # self.driver.get('https://pay.subless.com')
        self.driver.get('https://test-auth.subless.com/login?client_id=6a4425t6hjaerp2nndqo3el3d1&redirect_uri=https%3A%2F%2Fpay.subless.com%2Flogin&response_type=code&scope=openid&nonce=e98500393690007d289ed2b938ae0bd1e6SYkEEJR&state=606ee6c0d6c63ce241d89d69a944294c0bXYR96Rt&code_challenge=Wb4qFrOmGcqaWsz_73-Igz-N3dSDXyfim_sJRZIFXDw&code_challenge_method=S256')
        return self

    def enter_un(self, username):
        el = self.driver.find_element(By.ID, self.email_textbox_id)
        el.click()
        el.send_keys(username)

    def enter_pass(self, password):
        el = self.driver.find_element(self.PASS_FIELD)
        el.click()
        el.send_keys(password)

    def click_sign_up(self):
        el = self.driver.find_element(self.SIGNUP_LINK)
        el.click()

    def click_forgot_pass(self):
        el = self.driver.find_element(self.FORGOT_PASS_LINK)
        el.click()

    def click_google_signin(self):
        el = self.driver.find_element(self.GOOGLE_BUTTON)
        el.click()

    def click_sign_in(self):
        el = self.driver.find_element(self.SIGN_IN_BUTTON)
        el.click()

    def sign_in(self, un, password):
        self.enter_un(un)
        self.enter_pass(password)
        self.click_sign_in()

