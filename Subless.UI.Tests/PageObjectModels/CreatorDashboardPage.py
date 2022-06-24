import logging
import os
import time

from selenium.webdriver.support.wait import WebDriverWait

from .NavbarPage import NavbarPage

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger()


class CreatorDashboardPage(NavbarPage):

    def get_hit_count(self):
        logging.info("Waiting for stat request to finish")
        time.sleep(3)
        hit_count = self.driver.find_element_by_css_selector(CreatorDashboardLocators.hit_count_selector)
        logger.info("Hits on dashboard" + hit_count.text)
        return hit_count.text


class CreatorDashboardLocators:
    hit_count_selector = 'mat-card.mat-card:nth-child(1) > div:nth-child(2) > mat-card-content:nth-child(2)'

