"use strict";
/// <reference types="node" />
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
require('dotenv').config();
const restify = require("restify");
const builder = require("botbuilder");
const cards = require("./services/cards");
// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function () {
    console.log('%s listening to %s', server.name, server.url);
});
// Create chat connector for communicating with the Bot Framework Service
var connector = new builder.ChatConnector({
    appId: process.env.MICROSOFT_APP_ID,
    appPassword: process.env.MICROSOFT_APP_PASSWORD
});
// Listen for messages from users 
server.post('/api/messages', connector.listen());
// main dialog
var bot = new builder.UniversalBot(connector, [
    function (session) {
        session.send("Hey! I'm Ringo, the music bot 😎🎧🎵");
        builder.Prompts.text(session, "What's your name?");
    },
    function (session, results) {
        session.dialogData.name = results.response;
        session.send(`'Sup ${session.dialogData.name}! I love to discover new music and share my discoveries ;) `);
        //+ "I'm not really very smart so you may have to be patient with me :) If I start bugging out, just type 'help'.");
        session.beginDialog('fave_artists');
    },
    function (session, results) {
        session.endDialog();
    }
]);
// help dialog
// The dialog stack is cleared and this dialog is invoked when the user enters 'help'.
bot.dialog('help', function (session, args, next) {
    session.endDialog("Ringo is a bot that aims to discover music by asking you a series of questions about your music tastes."
        + "You can type 'quit' at any time to quit and resume the conversation later.");
})
    .triggerAction({
    matches: /^help$/i,
    onSelectAction: (session, args, next) => {
        // Add the help dialog to the dialog stack 
        // (override the default behavior of replacing the stack)
        session.beginDialog(args.action, args);
    }
});
// quit dialog
bot.dialog('quit', function (session, args, next) {
    session.endDialog("OK - see ya!");
})
    .triggerAction({
    matches: /^cancel$|^goodbye$|^quit$|^end$/i,
});
bot.dialog('fave_artists', [
    function (session, results) {
        builder.Prompts.text(session, "Who are your favourite artists and bands?");
    },
    function (session, results) {
        return __awaiter(this, void 0, void 0, function* () {
            var artists = results.response.split(',');
            session.dialogData.artists = artists;
            session.sendTyping();
            try {
                let msg = yield cards.getArtists(session, artists);
                if (msg)
                    session.send(msg);
                else {
                    session.send(`I couldn't find anything for "${results.response}" 😞 Try using commas, like "Lorde, Taylor Swift"`);
                    session.beginDialog('fave_artists'); //TODO IS this correct?
                }
            }
            catch (e) {
                console.error(e);
                session.send(`Whoops! Something is wrong 😞 Please try again.`);
                session.beginDialog('fave_artists');
            }
        });
    }
]);
