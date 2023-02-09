import logging

from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait
from selenium.webdriver.support import expected_conditions as EC

from PageObjectModels.LoginPage import LoginPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class TestSite_UriContentPage(object):

    def __init__(self, driver):
        self.driver = driver

    def open(self, username="TestUser"):
        self.driver.get(f'https://pythonclient{os.environ["environment"]}.subless.com/uriContent/'+username)
        return self