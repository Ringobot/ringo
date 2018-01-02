// https://docs.microsoft.com/en-us/bot-framework/nodejs/bot-builder-nodejs-state-azure-table-storage
import _azure = require('botbuilder-azure');

export function getAzureBotStorage() {
    let azureTableClient = new _azure.AzureTableClient('BotStateData', process.env.AZURE_STORAGE_ACCOUNT,
        process.env.AZURE_STORAGE_ACCESS_KEY);
    let tableStorage = new _azure.AzureBotStorage({ gzipData: false }, azureTableClient);
    return tableStorage;
}
