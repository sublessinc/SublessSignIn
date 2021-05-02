#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:7070
EXPOSE 7070

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
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

FROM build AS publish
RUN dotnet publish "./SublessSignIn/SublessSignIn.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./SublessSignIn/wwwroot /app/wwwroot
ENTRYPOINT ["dotnet", "SublessSignIn.dll"]
