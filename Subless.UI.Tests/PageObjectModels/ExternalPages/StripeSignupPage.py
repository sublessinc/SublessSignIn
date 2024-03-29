# todo:  fill this out
import time

from selenium.common.exceptions import ElementNotInteractableException
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as ec
from selenium.webdriver.common.by import By
from PageObjectModels.BasePage import BasePage, check_exists_by_xpath, check_exists_by_selector
from selenium.webdriver.common.action_chains import ActionChains


class StripeSignupPage(BasePage):
    @property
    def email_textbox(self):
        return self.driver.find_element_by_id(StripeSignupLocators.email_text_id)

    @property
    def cc_num_textbox(self):
        return self.driver.find_element_by_id(StripeSignupLocators.cc_num_id)

    @property
    def cc_expiry_textbox(self):
        return self.driver.find_element_by_id(StripeSignupLocators.cc_expiry_id)

    @property
    def cvc_num_textbox(self):
        return self.driver.find_element_by_id(StripeSignupLocators.cvc_id)

    @property
    def name_textbox(self):
        return self.driver.find_element_by_id(StripeSignupLocators.name_text_id)

    @property
    def zip_textbox(self):
        return self.driver.find_element_by_id(StripeSignupLocators.zip_text_id)

    @property
    def save_info_checkbox(self):
        return self.driver.find_element_by_css_selector(StripeSignupLocators.save_info_css_selector)

    @property
    def subscribe_button(self):
        if check_exists_by_selector(StripeSignupLocators.submit_button_selector, self.driver):
            return self.driver.find_element_by_css_selector(StripeSignupLocators.submit_button_selector)
        raise Exception("Subscribe button not found, stipe probably changed the page again.")

    def sign_up_for_stripe(self):
        from PageObjectModels.PatronDashboardPage import PatronDashboardPage
        WebDriverWait(self.driver, 10) \
            .until(ec.presence_of_element_located((By.ID, StripeSignupLocators.email_text_id)))
        self.email_textbox.send_keys('424@foo.bar')
        self.cc_num_textbox.send_keys('4242424242424242')
        self.cc_expiry_textbox.send_keys('424')
        self.cvc_num_textbox.send_keys('424')
        self.name_textbox.send_keys('Foo Bar')
        self.zip_textbox.send_keys('42424')
        if self.save_info_checkbox.is_selected():
            self.save_info_checkbox.click()
        WebDriverWait(self.driver, 10).until(
            ec.element_to_be_clickable((By.CSS_SELECTOR, StripeSignupLocators.submit_button_selector)))
        # this page is a jerk so we have to click into the button and let it change state
        try:
            self.subscribe_button.click()
        except ElementNotInteractableException:
            time.sleep(3)
            self.subscribe_button.click()
        WebDriverWait(self.driver, 10).until(lambda driver: 'stripe' not in self.driver.current_url)
        return PatronDashboardPage(self.driver)


class StripeSignupLocators:
    email_text_id = 'email'
    cc_num_id = 'cardNumber'
    cc_expiry_id = 'cardExpiry'
    cvc_id = 'cardCvc'
    name_text_id = 'billingName'
    zip_text_id = 'billingPostalCode'
    subscribe_button_xpath = '/html/body/div[1]/div/div[2]/div[2]/div/div[2]/form/div[2]/div[2]/button'
    subscribe_button_xpath2 = '/html/body/div[1]/div/div[2]/div[2]/div/div[2]/form/div[2]/div/div[2]/button'
    save_info_css_selector = '#enableStripePass'
    submit_button_selector = '.SubmitButton'
