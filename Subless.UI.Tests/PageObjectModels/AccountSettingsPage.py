from PageObjectModels.NavbarPage import NavbarPage


class AccountSettingsPage(NavbarPage):
    @property
    def cancel_subscription_button(self):
        return self.driver.find_element_by_id(AccountSettingsLocators.cancel_subscription_id)

class AccountSettingsLocators:
    cancel_subscription_id = ""