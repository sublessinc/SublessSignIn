import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage
from PageObjectModels.PlanSelectionPage import PlanSelectionPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class TermsPage(BasePage):

    @property
    def terms_link(self):
        return self.driver.find_element_by_css_selector(TermsLocators.terms_link_selector)

    @property
    def accept_terms_button(self):
        return self.driver.find_element_by_css_selector(TermsLocators.accept_selector)

    def accept_terms(self):
        logger.info(f'accepting terms')
        WebDriverWait(self.driver, 10).until(
            EC.element_to_be_clickable((By.CSS_SELECTOR, TermsLocators.accept_selector)))
        self.accept_terms_button.click()
        WebDriverWait(self.driver, 10).until(lambda driver: 'terms' not in driver.current_url)
        return PlanSelectionPage(self.driver)

class TermsLocators:
    terms_link_selector = 'a[href^="https://www.subless.com"]'
    accept_selector = 'button.mat-focus-indicator'
