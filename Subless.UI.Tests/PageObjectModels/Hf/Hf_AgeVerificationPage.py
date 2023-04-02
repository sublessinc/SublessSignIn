from PageObjectModels.Hf.Hf_HomePage import DevHf_HomePage


class DevHf_AgeVerificationPage(object):
    @property
    def enter_button(self):
        return self.driver.find_element_by_css_selector(DevHf_AgeVerificationPage_Locators.enter_button_selector)

    def open(self):
        self.driver.get(DevHf_AgeVerificationPage_Locators.dev_uri)
        return self

    def enter(self):
        self.enter_button.click()
        return DevHf_HomePage(self.driver)

    def __init__(self, driver):
        self.driver = driver

class DevHf_AgeVerificationPage_Locators:
    enter_button_selector = '#frontPage'
    dev_uri = f'https://dev.hentai-foundry.com/'