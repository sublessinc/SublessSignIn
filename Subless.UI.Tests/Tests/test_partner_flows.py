import pytest

from ApiLib.Admin import get_user_capabilities
from ApiLib.Partner import register_partner

@pytest.mark.skip("not yet implemented")
def test_register_partner_as_admin(subless_god_account, subless_account):
    # GIVEN: A Subless Admin Account
    god_id, god_token = subless_god_account

    # AND: A Subless account
    user_id, user_token = subless_account

    # WHEN: I register that user as a partner
    register_partner(user_id, god_token)

    # THEN: That user should have partner permissions
    user = get_user_capabilities(user_id, god_token)
    assert len(user['partners']) == 1
    assert user['partners'][0]['admin'] == user_id


def test_register_partner_as_non_admin(subless_account):
    pass


def test_register_self_as_partner(subless_admin_account):
    pass


def test_register_self_as_partner_as_non_admin(subless_account):
    pass

def test_register_multiple_partners():
    pass