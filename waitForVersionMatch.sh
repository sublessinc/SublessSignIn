short=$(git rev-parse --short HEAD)
for i in {1..20}
do
  echo "Checking if version deployed - test $i"
  response=$(curl -sb -H "Accept: application/json" "https://dev.subless.com/api/version")
  if [[ $response == *$short* ]]
  then
    echo "version match"
    exit 1
  fi
  echo "deployed version"
  echo $response
  sleep 5s
done
echo "Environment version did not update to match current branch" >&2