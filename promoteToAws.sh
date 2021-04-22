aws ecr get-login-password --region us-east-2 | docker login --username AWS --password-stdin 548668861663.dkr.ecr.us-east-2.amazonaws.com
docker build -t subless-users .
docker tag subless-users:latest 548668861663.dkr.ecr.us-east-2.amazonaws.com/sublesspayment:latest
docker push 548668861663.dkr.ecr.us-east-2.amazonaws.com/sublesspayment:latest