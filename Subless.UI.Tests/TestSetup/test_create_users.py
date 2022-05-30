import pytest

import ApiLib.User
from EmailLib import MailSlurp
from PageObjectModels.LoginPage import LoginPage
from UsersLib.Users import create_user, get_all_test_user_data, save_user_test_data, get_user_id_and_cookie

usertypes = [
    'AdminUser',
    'PartnerUser',
    'CreatorUser',
    'BasicUser',
]

@pytest.mark.parametrize('usertype', usertypes)
def test_create_test_users(usertype, firefox_driver, user_data):
    inbox = MailSlurp.get_or_create_inbox(usertype)

    id, token = create_user(firefox_driver, inbox)

    user_data[usertype] = {'id': id,
                           'email': inbox.email_address,
                           'token': token}

@pytest.mark.skip("not yet implemented")
def test_retrieve_god_user_token(firefox_driver, user_data):
    pass


@pytest.mark.parametrize('usertype', usertypes)
def test_delete_all_users(usertype):
    data = get_all_test_user_data()
    ApiLib.User.delete(data[usertype]['token'])


@pytest.mark.parametrize('usertype', usertypes)
def test_delete_users_smart(usertype, firefox_driver):
    login_page = LoginPage(firefox_driver).open()

    login_page.sign_in(MailSlurp.get_or_create_inbox(usertype).email_address, 'SublessTestUser')

    id, token = get_user_id_and_cookie(firefox_driver)

    login_page.logout()

    ApiLib.User.delete(token)
