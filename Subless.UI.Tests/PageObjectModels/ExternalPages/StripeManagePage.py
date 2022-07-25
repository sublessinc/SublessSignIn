# todo:  fill this out
from selenium.common.exceptions import NoSuchElementException

from PageObjectModels.BasePage import BasePage


class StripeManagementPage(BasePage):

    @property
    def current_plan_text(self):
        # there are two possible selectors, and this seems to

        if (self.driver.find_elements_by_css_selector(StripeManagementLocators.current_plan_selector)):
            return self.driver.find_element_by_css_selector(StripeManagementLocators.current_plan_selector)
        return self.driver.find_element_by_css_selector(StripeManagementLocators.current_plan_selector2)
class StripeManagementLocators:
    current_plan_selector = ".Flex-alignItems--flexStart > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > span:nth-child(1) > span:nth-child(1)"
    current_plan_selector2 = ".Flex-alignItems--flexStart > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(3) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > span:nth-child(1) > span:nth-child(1)"
