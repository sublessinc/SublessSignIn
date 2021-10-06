while getopts e: flag
do
    case "${flag}" in
        e) environment=${OPTARG};;
    esac
done
if [ -z "$environment" ]; then
  echo 1>&2 "Environment flag missing"
  exit 2
fi
echo "environment: environment";

cd ./Subless.UI/sublessui
ng build --configuration $environment && cp -r ./dist/sublessui/* ../../SublessSignIn/wwwroot
cd ./../../
cd ./Subless.JS/
npm run build:$environment
cd ./../
aws ecr get-login-password --region us-east-1 | docker login --username AWS --password-stdin 548668861663.dkr.ecr.us-east-1.amazonaws.com
docker build -t subless-pay:$environment .
docker tag subless-pay:$environment 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-pay:$environment
docker push 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-pay:$environment


docker build -f CalculatorDockerfile . -t subless-calculator:$environment
docker tag subless-calculator:$environment 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-calculator:$environment
docker push 548668861663.dkr.ecr.us-east-1.amazonaws.com/subless-calculator:$environment