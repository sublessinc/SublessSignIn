# todo:  fill this out
from PageObjectModels.BasePage import BasePage


class StripeManagementPage(BasePage):

    @property
    def current_plan_text(self):
        return self.driver.find_element_by_css_selector(StripeManagementLocators.current_plan_selector)



class StripeManagementLocators:
    current_plan_selector = ".Flex-alignItems--flexStart > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(2) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > div:nth-child(1) > span:nth-child(1) > span:nth-child(1)"
