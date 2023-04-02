from Keys.Keys import Keys


class Hf_CreatorPage(object):
    @property
    def subless_login(self):
        return self.driver.find_element_by_css_selector(Hf_CreatorPage_locators.subless_login_selector)

    @property
    def subless_logout(self):
        return self.driver.find_element_by_css_selector(Hf_CreatorPage_locators.subless_logout_selector)

    def open(self):
        self.driver.get(Hf_CreatorPage_locators.uri)
        return self

    def __init__(self, driver):
        self.driver = driver

class Hf_CreatorPage_locators:
    uri = Keys.hf_uri + '/pictures/user/SublessAutomationTestUser'
