import logging
import os
import time

from selenium.webdriver.support.wait import WebDriverWait

from .NavbarPage import NavbarPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class PatronDashboardPage(NavbarPage):
    @property
    def manage_billing_button(self):
        return self.driver.find_element_by_xpath(PatronDashboardLocators.manage_billing_button_xpath)

    @property
    def user_profile_button(self):
        return self.driver.find_element_by_id(PatronDashboardLocators.user_profile_button_id)

    def go_to_manage_billing(self):
        self.manage_billing_button.click()
        # todo: return manage billing page

    def view_user_profile(self):
        self.user_profile_button.click()
        # todo:  I don't know what's supposed to happen here-- might not make sense to be part of this POM

    def get_hit_count(self):
        time.sleep(3)
        hit_count = self.driver.find_element_by_css_selector(PatronDashboardLocators.hit_count_selector)
        logger.info("Hits on dashboard" + hit_count.text)
        return hit_count.text

    def open(self):
        self.driver.get(f'https://{os.environ["environment"]}.subless.com/user-profile')
        WebDriverWait(self.driver, 10).until(lambda driver: 'subless' in driver.title)
        return self

class PatronDashboardLocators:
    manage_billing_button_xpath = '//*[@id="bodyWrapper"]/div[2]/div/div[2]/button'
    user_profile_button_id = 'user'
    hit_count_selector = '#root > mat-sidenav-content > app-userprofile > div > mat-card:nth-child(1) > div.statText > mat-card-content.mat-card-content.numberText'

