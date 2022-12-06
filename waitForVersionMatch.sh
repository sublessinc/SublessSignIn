short=$(git rev-parse --short HEAD)
for i in {1..20}
do
  echo "Checking if version deployed - test $i"
  response=$(curl -sb -H "Accept: application/json" "https://dev.subless.com/api/version")
  if [[ $response == *$short* ]]
  then
    echo "version match"
    exit 0
  fi
  echo "deployed version"
  echo $response
  sleep 30s
done
echo "Environment version did not update to match current branch" >&2
exit 1