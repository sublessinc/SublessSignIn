
python -m venv virtual_env
call virtual_env\Scripts\activate.bat
pip install --trusted-host https://pypi.python.org -r requirements.txt

python -m pytest

deactivate