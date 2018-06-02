# Ringo

<https://ringobot.azurewebsites.net/>

Ringo is a Chat Bot by @DanielLarsenNZ and @msimpsonnz that learns what music you
like and helps you to discover more. It demonstrates several technologies including:

* Microsoft Bot Framework
* Nodejs + TypeScript
* Microsoft CosmosDB, Graph DB
* .NET Core in Azure Functions
* Spotify Web API
* Azure App Services, Table Storage, Service Bus

## Getting started

To get the bot working on your local machine:

    git clone https://github.com/Ringobot/ringo.git
    cd ./ringo/src/Ringo.Bot
    npm install
    touch .env

Copy the following settings into a dotenv file, replacing the `<PLACEHOLDERS>` with
your own values:

```text
SpotifyApiClientId=<SPOTIFY-API-CLIENTID>
SpotifyApiClientSecret=<SPOTIFY-API-CLIENTSECRET>
AZURE_STORAGE_ACCOUNT=<STORAGE-ACCOUNT-NAME>
AZURE_STORAGE_ACCESS_KEY=<STORAGE-ACCOUNT-KEY>
GRAPH_ENDPOINT=<GRAPH-ENDPOINT>
GRAPH_ACCESS_KEY=<GRAPH-ACCESS-KEY>
GRAPH_DATABASE_ID=<GRAPH-DATABASE-ID>
GRAPH_COLLECTION_ID=<GRAPH-COLLECTION-ID>
LUIS_MODEL_URL=<LUIS-MODEL-URL>
APPINSIGHTS_INSTRUMENTATIONKEY=<APPINSIGHTS-INSTRUMENTATIONKEY>
SpotifyAuthRedirectUri=http://localhost:3978/authorize/spotify
SB_CONNECTION_STRING=Endpoint=sb://ringobotsb.servicebus.windows.net/;SharedAccessKeyName=BotSend;SharedAccessKey=<SHARED-ACCESS-KEY>
```

> Tip: `GRAPH_ENDPOINT` should be specified as a DNS name without a protocol, e.g.
> `mygraphdb.gremlin.cosmosdb.azure.com`

### List of links to prerequisites

Register for a Spotify API Key: <https://developer.spotify.com/dashboard/>

Get a free Azure Account: <https://azure.microsoft.com/en-us/free/startups/>

Create a Storage Account: <https://azure.microsoft.com/en-us/services/storage/>





## Deployment

1. Commit / merge your changes to `master`
1. Push to VSTS
1. Create a new **Deploy Ringo** Release in VSTS

Deployment process will deploy to staging slot, run `npm test` and then swap.

### Deployment links

Set up staging environments in Azure App Service: <https://docs.microsoft.com/en-us/azure/app-service/web-sites-staged-publishing>

Best practices for using App Service Local Cache: <https://docs.microsoft.com/en-us/azure/app-service/app-service-local-cache-overview#best-practices-for-using-app-service-local-cache>

Speeding up Azure App Service Builds: <https://wildermuth.com/2017/01/14/Speeding-up-Azure-App-Service-Builds>

## Bot Framework

Connect a bot to Slack: <https://docs.microsoft.com/en-us/azure/bot-service/bot-service-channel-connect-slack>

Channel inspector: <https://docs.botframework.com/en-us/channel-inspector/channels/Slack?f=Carousel&e=example1>

Custom recognizer: <https://docs.microsoft.com/en-us/azure/bot-service/nodejs/bot-builder-nodejs-recognize-intent-messages#register-a-custom-intent-recognizer>

## Restify

Restify cheatsheet: <https://gist.github.com/LeCoupa/0664e885fd74152d1f90>

Restify errors: <https://github.com/restify/errors>

## Spotify

Spotify Developer: <https://developer.spotify.com/web-api/console/get-search-item/#complete>

Authorisation guide: <https://beta.developer.spotify.com/documentation/general/guides/authorization-guide/#authorization-code-flow>

## Typescript

Downlevel Async Functions: <https://www.typescriptlang.org/docs/handbook/release-notes/typescript-2-1.html>

Unhandled Promise Rejections in Node.js: <http://thecodebarbarian.com/unhandled-promise-rejections-in-node.js.html>

The 80/20 Guide to Async/Await in Node.js: <http://thecodebarbarian.com/80-20-guide-to-async-await-in-node.js.html>

How to get node.js HTTP request promise without a single dependency: <https://www.tomas-dvorak.cz/posts/nodejs-request-without-dependencies/>

Regex tester: <https://regex101.com/>

### Testing and Mocha

Unit testing node applications with TypeScript — using mocha and chai: <https://journal.artfuldev.com/unit-testing-node-applications-with-typescript-using-mocha-and-chai-384ef05f32b2>

Unit testing a TypeScript library: <https://www.tsmean.com/articles/how-to-write-a-typescript-library/unit-testing/>

Fake Online REST API for Testing and Prototyping: <https://jsonplaceholder.typicode.com/>

Using mocha programmatically <https://github.com/mochajs/mocha/wiki/Using-mocha-programmatically>

Mocha async: <https://mochajs.org/#asynchronous-code>

## Linux, Docker, Nodejs

Install latest nodejs and npm (on Ubuntu): <https://nodejs.org/en/download/package-manager/#debian-and-ubuntu-based-linux-distributions>

    curl -sL https://deb.nodesource.com/setup_8.x | sudo -E bash - sudo apt-get install -y nodejs

Dockerizing a Node.js web app: <https://nodejs.org/en/docs/guides/nodejs-docker-webapp/>

## Application insights

<https://github.com/Microsoft/ApplicationInsights-node.js/>

How to track auth users: <https://github.com/Microsoft/ApplicationInsights-node.js/issues/356>

## More Links

Emojipedia: <https://emojipedia.org/people/>
