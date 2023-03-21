class ExistingUserException(Exception):
    """Exception for trying to create a user that already exists"""


class ApiLimitException(Exception):
    """ Exception for when Amazon hits API limits """

