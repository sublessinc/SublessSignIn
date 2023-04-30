import logging
import time

import pytest
from selenium.webdriver.common.by import By
from selenium.webdriver.support.wait import WebDriverWait

from Keys.Keys import Keys
from PageObjectModels.Hf.Hf_AgeVerificationPage import DevHf_AgeVerificationPage, \
    DevHf_AgeVerificationPage_Locators
from selenium.webdriver.support import expected_conditions as EC

from PageObjectModels.Hf.Hf_CreatorPage import Hf_CreatorPage
from PageObjectModels.Hf.Hf_HomePage import DevHf_HomePage_Locators
from PageObjectModels.PatronDashboardPage import PatronDashboardPage


@pytest.mark.skipif("stage" not in Keys.subless_uri, reason="an HF enviornment only exists for stage")
def test_no_user_login_button_on_partner_site(web_driver, params):
    # WHEN: I visit a partner
    hf_age_verfication = DevHf_AgeVerificationPage(web_driver)
    hf_age_verfication.open()
    logging.info("Waiting hf to load, and login check to run")
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_AgeVerificationPage_Locators.enter_button_selector)))
    hf_home_page = hf_age_verfication.enter()
    # THEN: I should not be logged in
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_HomePage_Locators.subless_login_selector)))
    assert hf_home_page.subless_login.is_displayed()
    assert not hf_home_page.subless_logout.is_displayed()


@pytest.mark.skipif("stage" not in Keys.subless_uri, reason="an HF enviornment only exists for stage")
def test_logged_in_user_logout_button_on_partner_site(web_driver, paying_user, params):
    # WHEN: I visit a partner
    hf_age_verfication = DevHf_AgeVerificationPage(web_driver)
    hf_age_verfication.open()
    logging.info("Waiting hf to load, and login check to run")
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_AgeVerificationPage_Locators.enter_button_selector)))
    hf_home_page = hf_age_verfication.enter()
    # I have to click login b/c firefox security settings
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_HomePage_Locators.subless_login_selector)))
    hf_home_page.subless_login.click()
    # THEN: I should be logged in
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_HomePage_Locators.subless_logout_selector)))
    assert not hf_home_page.subless_login.is_displayed()
    assert hf_home_page.subless_logout.is_displayed()


@pytest.mark.skipif("stage" not in Keys.subless_uri, reason="an HF enviornment only exists for stage")
def test_paying_user_pushes_hit_on_partner_site(web_driver, paying_user, params):
    dashboard = PatronDashboardPage(web_driver).open()
    hits_before = dashboard.get_hit_count()
    hf_age_verification = DevHf_AgeVerificationPage(web_driver)
    hf_age_verification.open()
    logging.info("Waiting hf to load, and login check to run")
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_AgeVerificationPage_Locators.enter_button_selector)))
    hf_home_page = hf_age_verification.enter()
    # I have to click login b/c firefox security settings
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_HomePage_Locators.subless_login_selector)))
    hf_home_page.subless_login.click()
    # THEN: I should be logged in
    WebDriverWait(web_driver, 20).until(
        EC.element_to_be_clickable((By.CSS_SELECTOR, DevHf_HomePage_Locators.subless_logout_selector)))
    assert not hf_home_page.subless_login.is_displayed()
    assert hf_home_page.subless_logout.is_displayed()
    creator_page = Hf_CreatorPage(web_driver)
    creator_page.open()
    # wait for hit to push
    time.sleep(5)
    dashboard = PatronDashboardPage(web_driver).open()
    hits_after = dashboard.get_hit_count()
    assert int(hits_after) - int(hits_before) == 1
