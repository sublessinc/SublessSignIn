# todo:  fill this out
from selenium.common.exceptions import NoSuchElementException
from selenium.webdriver.common.by import By
from selenium.webdriver.support import expected_conditions as EC
from selenium.webdriver.support.wait import WebDriverWait

from PageObjectModels.BasePage import BasePage


class StripeManagementPage(BasePage):

    @property
    def current_plan_text(self):
        # there are two possible selectors, and this seems to
        try:
            WebDriverWait(self.driver, 10).until(
                EC.visibility_of_element_located((By.CSS_SELECTOR, StripeManagementLocators.current_plan_selector3)))
            return self.driver.find_element_by_css_selector(StripeManagementLocators.current_plan_selector3)
        except:
            WebDriverWait(self.driver, 10).until(
                EC.visibility_of_element_located((By.CSS_SELECTOR, StripeManagementLocators.current_plan_selector2)))
            return self.driver.find_element_by_css_selector(StripeManagementLocators.current_plan_selector2)

class StripeManagementLocators:
    current_plan_selector = ".Flex-alignItems--flexStart > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > span:nth-child(1) > span:nth-child(1)"
    current_plan_selector2 = ".Flex-alignItems--flexStart > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > span:nth-child(1) > span:nth-child(1)"
    current_plan_selector3 = "span.Text-color--default:nth-child(1) > span:nth-child(1)"
    "span.Text-color--default:nth-child(1) > span:nth-child(1)"