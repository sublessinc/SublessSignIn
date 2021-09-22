from ApiLib import Admin
from ApiLib.Admin import get_user_capabilities


def test_make_user_admin(subless_account, subless_god_account):
    # GIVEN: I have a subless account
    #          (handled by fixture)
    god_id, god_token = subless_god_account
    id, token = subless_account

    # WHEN: I call the setadmin endpoint
    Admin.set_admin(id, god_token)

    # THEN: My user should have admin permissions
    user = get_user_capabilities(id, god_token)
    assert user['isAdmin']
