#!/bin/bash
python3 -m venv virtual_env --clear
source virtual_env/bin/activate
pip install --trusted-host https://pypi.python.org -r requirements.txt

python3 -m pytest Tests

deactivate