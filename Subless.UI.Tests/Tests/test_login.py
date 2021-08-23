from Fixtures.drivers import chrome_driver
from PageObjectModels.LoginPage import LoginPage
import logging

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


def test_login(chrome_driver):
    login_page = LoginPage(chrome_driver).open()
    login_page.sign_in('x', 'x')
    assert "Sublessui" in chrome_driver.title
    assert 'register-payment' in chrome_driver.current_url
