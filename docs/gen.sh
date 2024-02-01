#!/bin/bash

SERVICES=(Shked-Authorization Shked-GroupsService Shked-StorageService Shked-TasksService Shked-UsersService)
CUR_DIR=$(dirname $0)
SDK_VERSION=$( dotnet --list-sdks | awk 'NR==1{print $1}' )

cd $CUR_DIR

cd ../src

if [ -z "$(ls -a | grep .config )" ]; then
    dotnet new tool-manifest
fi

if [ -z "$(dotnet tool list | grep Swashbuckle.AspNetCore.Cli )" ]; then
    dotnet tool install Swashbuckle.AspNetCore.Cli
fi

dotnet new globaljson --force --sdk-version $SDK_VERSION --roll-forward latestMinor

for item in ${SERVICES[*]}
do
  echo "Generating docs for $item project..."
  cd $item || continue 
  dotnet restore $item.csproj > /dev/null || continue 
  dotnet publish "$item.csproj" -c Release -o ./bin/Publish /p:UseAppHost=false > /dev/null || continue
  dotnet swagger tofile --yaml --output ./../../docs/$item.yaml bin/Publish/$item.dll v1
  cd ..
done

