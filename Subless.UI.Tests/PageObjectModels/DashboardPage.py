import logging
from .NavbarPage import NavbarPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class DashboardPage(NavbarPage):
    @property
    def manage_billing_button(self):
        return self.driver.find_element_by_xpath(DashboardLocators.manage_billing_button_xpath)

    @property
    def user_profile_button(self):
        return self.driver.find_element_by_id(DashboardLocators.user_profile_button_id)

    def go_to_manage_billing(self):
        self.manage_billing_button.click()
        # todo: return manage billing page

    def view_user_profile(self):
        self.user_profile_button.click()
        # todo:  I don't know what's supposed to happen here-- might not make sense to be part of this POM


class DashboardLocators:
    manage_billing_button_xpath = '//*[@id="bodyWrapper"]/div[2]/div/div[2]/button'
    user_profile_button_id = 'user'


