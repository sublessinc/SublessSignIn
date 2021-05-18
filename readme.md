# Subless

Subless sign in manages user data for subless

[Architecture](https://app.diagrams.net/#G1cJIMwMpMkj7GtDjS6SLBZVKdjGs1rUSZ)
## Build
### Requirements
Docker

dotnet core

powershell

dotnet tool install --global dotnet-ef


### Debug

Start postgresql
```bash
docker run -e POSTGRES_PASSWORD=[password] -p 5432:5432 postgres
```
Define environment variables
```bash
STRIPE_PUBLISHABLE_KEY=
BASIC_PRICE_ID=
STRIPE_SECRET_KEY=
DOMAIN=
STRIPE_WEBHOOK_SECRET=
region=
appClientId=
userPoolId=
CONNECTION_STRING=
```

Run application

```bash
dotnet run --project SublessSignIn/SublessSignIn.csproj
```

Browse to [http://localhost:7070/](http://localhost:7070/)

## Deploy

Run
```bash
promoteToAws.sh
```
Stop [the current running task](https://console.aws.amazon.com/ecs/home?region=us-east-1#/clusters/SublessPayments/services/paysubless/tasks)

