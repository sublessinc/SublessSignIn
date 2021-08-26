cd ./Subless.UI/sublessui
ng build --configuration dev && cp -r ./dist/sublessui/* ../../SublessSignIn/wwwroot
cd ./../../
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 548668861663.dkr.ecr.us-east-1.amazonaws.com
docker build -t subless-pay:dev .
docker tag subless-pay:dev 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-pay:dev
docker push 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-pay:dev


docker build -f CalculatorDockerfile . -t subless-calculator:dev
docker tag subless-calculator:dev 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-calculator:dev
docker push 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-calculator:dev