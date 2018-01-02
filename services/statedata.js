"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// https://docs.microsoft.com/en-us/bot-framework/nodejs/bot-builder-nodejs-state-azure-table-storage
const _azure = require("botbuilder-azure");
function getAzureBotStorage() {
    let azureTableClient = new _azure.AzureTableClient('BotStateData', process.env.AZURE_STORAGE_ACCOUNT, process.env.AZURE_STORAGE_ACCESS_KEY);
    let tableStorage = new _azure.AzureBotStorage({ gzipData: false }, azureTableClient);
    return tableStorage;
}
exports.getAzureBotStorage = getAzureBotStorage;
