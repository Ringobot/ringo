"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const _azure = require("azure-sb");
var connStr = process.env.SB_CONNECTION_STRING;
var sbService = _azure.createServiceBusService(connStr);
if (!connStr)
    throw new Error('Env var SB_CONNECTION_STRING not set');
function sendMessage(topicName, messageBody) {
    return __awaiter(this, void 0, void 0, function* () {
        var message = {
            body: (JSON.stringify(messageBody))
        };
        console.log("serviceBus:sendMessage", message.body);
        return new Promise((resolve, reject) => {
            sbService.sendTopicMessage(topicName, message, function (err) {
                if (err) {
                    console.error(err);
                    reject(err);
                    return;
                }
                else {
                    resolve();
                    return;
                }
            });
        });
    });
}
exports.sendMessage = sendMessage;
