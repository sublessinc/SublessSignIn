import pytest

import ApiLib.User
from EmailLib import MailSlurp
from PageObjectModels.LoginPage import LoginPage
from UsersLib.Users import create_user, get_all_test_user_data, save_user_test_data, get_user_id_and_token

usertypes = [
    'AdminUser',
    'PartnerUser',
    'CreatorUser',
    'BasicUser',
]

@pytest.mark.parametrize('usertype', usertypes)
def test_create_test_users(usertype, firefox_driver, user_data):
    inbox = MailSlurp.get_inbox_from_name(usertype)

    id, token = create_user(firefox_driver, inbox)

    user_data[usertype] = {'id': id,
                           'email': inbox.email_address,
                           'token': token}


def test_retrieve_god_user_token(firefox_driver, user_data):
    from Keys.Keys import Keys

    login_page = LoginPage(firefox_driver).open()
    login_page.sign_in(Keys.god_email, Keys.god_password)

    id, token = get_user_id_and_token(firefox_driver)

    user_data['GodUser'] = {'id': id,
                            'email': Keys.god_email,
                            'token': token}


@pytest.mark.parametrize('usertype', usertypes)
def test_delete_all_users(usertype):
    data = get_all_test_user_data()
    ApiLib.User.delete(data[usertype]['token'])
