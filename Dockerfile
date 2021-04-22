#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:7070
EXPOSE 7070

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["./SublessSignIn.csproj", ""]
RUN dotnet restore "./SublessSignIn.csproj"

COPY . .
WORKDIR "/src/."
RUN dotnet build "./SublessSignIn.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "./SublessSignIn.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY ./wwwroot /app/wwwroot
ENTRYPOINT ["dotnet", "SublessSignIn.dll"]
