import unittest
from selenium import webdriver
from selenium.webdriver.common.keys import Keys
import time


class NewVisitorTests(unittest.TestCase):

    @classmethod
    def setUpClass(self):
        self.browser = webdriver.Firefox()

    def test_TC0001_user_can_register(self):

        # User navigates to EC
        self.browser.get("http://localhost:3000/")
        time.sleep(1)

        # She notices the company title
        self.assertIn("E C", self.browser.title)

        # User decides to register
        registerLink = self.browser.find_elements_by_id(
            "registerModalTrigger")[0]
        registerLink.click()
        time.sleep(1)

        usernameInput = self.browser.find_element_by_name("username")
        usernameInput.send_keys("FunctionalTester")
        time.sleep(1)

        passwordInput = self.browser.find_element_by_name("password")
        passwordInput.send_keys("#33FalleN666#")
        time.sleep(1)

        rePasswordInput = self.browser.find_element_by_name("rePassword")
        rePasswordInput.send_keys("#33FalleN666#")
        time.sleep(1)

        submitButton = self.browser.find_element_by_name("submit")
        submitButton.click()

    def test_TC0002_user_can_authenticate(self):

        # User navigates to EC
        self.browser.get("http://localhost:3000/")
        time.sleep(1)

        # She notices the company title
        self.assertIn("E C", self.browser.title)

        # User decides to authenticate
        signInLink = self.browser.find_elements_by_id(
            "loginModalTrigger")[0]
        signInLink.click()
        time.sleep(1)

        usernameInput = self.browser.find_element_by_name("username")
        usernameInput.send_keys("FunctionalTester")
        time.sleep(1)

        passwordInput = self.browser.find_element_by_name("password")
        passwordInput.send_keys("#33FalleN666#")
        time.sleep(1)

        submitButton = self.browser.find_element_by_name("submit")
        submitButton.click()

    @classmethod
    def tearDownClass(self):
        self.browser.quit()


if (__name__ == "__main__"):
    unittest.main()
