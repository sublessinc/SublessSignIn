from ApiLib import Admin
from ApiLib.Admin import get_user_capabilities


def test_make_user_admin(subless_account, user_data):
    # GIVEN: I have a subless account
    #          (handled by fixture)
    id, token = subless_account

    # WHEN: I call the setadmin endpoint
    Admin.set_admin(id, user_data['GodUser']['token'])

    # THEN: My user should have admin permissions
    user = get_user_capabilities(id, user_data['GodUser']['token'])
    assert user['isAdmin']
