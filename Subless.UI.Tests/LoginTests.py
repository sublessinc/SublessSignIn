import pytest
from selenium import webdriver
from Fixtures.drivers import chrome_driver
from PageObjectModels.LoginPage import LoginPage
from PageObjectModels.Google import Google
import logging
import time


def test_google(chrome_driver):
    google = Google(chrome_driver).open()
    google.enter_search('dick tree')
    time.sleep(10)

@pytest.mark.skip
def test_load_login(chrome_driver):
    login_page = LoginPage(chrome_driver).open()
    login_page.sign_in('dick tree', 'penis')
    time.sleep(3)
