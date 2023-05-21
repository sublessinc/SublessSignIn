# Subless

Subless sign in manages user data for subless

[Architecture](https://app.diagrams.net/#G1cJIMwMpMkj7GtDjS6SLBZVKdjGs1rUSZ)
## Build
### Requirements
Docker

dotnet core

npm

powershell

dotnet tool install --global dotnet-ef

python 3+

### Debug

Start postgresql
```bash
# password used can be found or set in the launchsettings.json file
docker run -e POSTGRES_PASSWORD=[password] -p 5432:5432 postgres
```
Define environment variables (this can be done in the CLI as below, or using the `launchSettings.json` file)
```bash
STRIPE_PUBLISHABLE_KEY=
STRIPE_SECRET_KEY=
DOMAIN=
STRIPE_WEBHOOK_SECRET=
region=
appClientId=
userPoolId=
CONNECTION_STRING=
```

Install npm dependencies

```bash
cd ./Subless.UI/sublessui/
npm install
```

Run back end

```bash
cd ../../
dotnet run --project SublessSignIn/SublessSignIn.csproj
```

Run front end
```bash
cd ./Subless.UI/sublessui/
npm start
```

Browse to [https://localhost:4200/](https://localhost:4200/)


Optional - Build Redistributable
```bash
cd ./Subless.JS
npm install
npm run build:local
```

## Test

```bash
cd ./Subless.UI.Tests/
bash run_tests.sh
```

## Deploy

Merge to the "dev" branch