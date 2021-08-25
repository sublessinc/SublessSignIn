cd ./Subless.UI/sublessui
npm run-script build
cd ./../../
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 548668861663.dkr.ecr.us-east-1.amazonaws.com
docker build -t subless-pay .
docker tag subless-pay:latest 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-pay:latest
docker push 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-pay:latest


docker build -f CalculatorDockerfile . -t subless-calculator
docker tag subless-calculator:latest 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-calculator:latest
docker push 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-calculator:latest