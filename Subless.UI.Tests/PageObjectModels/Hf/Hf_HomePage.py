from Keys.Keys import Keys


class DevHf_HomePage(object):
    @property
    def subless_login(self):
        return self.driver.find_element_by_css_selector(DevHf_HomePage_Locators.subless_login_selector)

    @property
    def subless_logout(self):
        return self.driver.find_element_by_css_selector(DevHf_HomePage_Locators.subless_logout_selector)

    @property
    def hf_username_textbox(self):
        return self.driver.find_element_by_css_selector(DevHf_HomePage_Locators.hf_username_textbox_selector)

    @property
    def hf_password_textbox(self):
        return self.driver.find_element_by_css_selector(DevHf_HomePage_Locators.hf_password_textbox_selector)

    @property
    def hf_login_button(self):
        return self.driver.find_element_by_css_selector(DevHf_HomePage_Locators.hf_login_selector)

    def login_to_hf(self, username, password):
        self.hf_username_textbox.send_keys(username)
        self.hf_password_textbox.send_keys(password)
        self.hf_login_button.click()

    def open(self):
        self.driver.get(DevHf_HomePage_Locators.dev_uri)
        return self

    def __init__(self, driver):
        self.driver = driver

class DevHf_HomePage_Locators:
    dev_uri = Keys.hf_uri + "/?enterAgree=1"
    subless_login_selector = "#sublessBtnLogin"
    subless_logout_selector = "#sublessBtnLogout"
    hf_username_textbox_selector = "#headerLogin > form > input[type=text]:nth-child(2)"
    hf_password_textbox_selector = "#headerLogin > form > input[type=password]:nth-child(3)"
    hf_login_selector = "#headerLogin > form > input[type=submit]:nth-child(4)"