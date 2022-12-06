#/bin/bash
export PATH="$PATH:/root/.dotnet/tools"
dotnet-gitversion | grep FullSemVer | awk '{ print  }' | sed 's/"//g' | sed 's/,//'  > version.txt
dotnet-gitversion | grep ShortSha | awk '{ print  }' | sed 's/"//g' | sed 's/,//'  >> version.txt