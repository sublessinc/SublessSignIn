#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.
ARG build_environment=dev


FROM node:16-alpine as angularbuild
ARG build_environment
RUN echo "building for $build_environment"
WORKDIR /src
RUN mkdir -p /src/build
COPY /Subless.UI/sublessui/package*.json ./
RUN npm install -g @angular/cli
RUN npm install
COPY  /Subless.UI/sublessui ./
RUN ng build --configuration $build_environment

FROM 548668861663.dkr.ecr.us-east-2.amazonaws.com/subless-js:$build_environment as jsbuild


FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:7070
EXPOSE 7070

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["./Subless.Data/Subless.Data.csproj", "./Subless.Data/"]
RUN dotnet restore "./Subless.Data/Subless.Data.csproj"
COPY ["./Subless.Models/Subless.Models.csproj", "./Subless.Models/"]
RUN dotnet restore "./Subless.Models/Subless.Models.csproj"
COPY ["./SublessSignIn/SublessSignIn.csproj", "./SublessSignIn/"]
RUN dotnet restore "./SublessSignIn/SublessSignIn.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "./SublessSignIn/SublessSignIn.csproj" -c Release -o /app/build

FROM build AS test
RUN dotnet test

FROM build AS publish
RUN dotnet publish "./SublessSignIn/SublessSignIn.csproj" -c Release -o /app/publish

FROM build AS version
RUN dotnet tool install --global GitVersion.Tool --version 5.*
RUN ./version.sh

FROM base AS final
WORKDIR /app
RUN mkdir -p /app/wwwroot/dist/assets
COPY --from=publish /app/publish .
COPY --from=version /src/version.txt .
COPY ./SublessSignIn/wwwroot /app/wwwroot
COPY --from=angularbuild /src/dist/sublessui /app/wwwroot
COPY --from=angularbuild /src/dist/sublessui/assets/redist /app/wwwroot/dist/assets
COPY --from=jsbuild /src/dist /app/wwwroot/dist
ENTRYPOINT ["dotnet", "SublessSignIn.dll"]

