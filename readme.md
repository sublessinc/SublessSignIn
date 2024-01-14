# Subless

Subless sign in manages user data for subless

[Architecture](https://app.diagrams.net/#G1cJIMwMpMkj7GtDjS6SLBZVKdjGs1rUSZ)
## Build
### Requirements

- Docker
- dotnet core
- npm
- powershell
- To have run `dotnet tool install --global dotnet-ef`
- python 3+

### Debug

You'll need to sign into aws cli. You can use the installer [here](https://docs.aws.amazon.com/cli/latest/userguide/getting-started-install.html).
This will set up your environment with a ~/.aws/credentials file with the
necessary credentials.

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

## License

Subless' source is MIT licensed:

Copyright 2023 Subless, Inc.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the “Software”), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED “AS IS”, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
