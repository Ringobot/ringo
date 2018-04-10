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
const appInsights = require("applicationinsights");
appInsights.setup();
appInsights.start();
const restify = require("restify");
const builder = require("botbuilder");
const _artists = require("./services/artists");
const _cards = require("./services/cards");
const _messages = require("./services/messages");
const userdata = require("./services/userdata");
const statedata = require("./services/statedata");
const helpers = require("./helpers");
// Setup Restify Server
var server = restify.createServer();
server.listen(process.env.port || process.env.PORT || 3978, function (e) {
    if (e)
        throw e;
    console.log('%s listening to %s', server.name, server.url);
});
server.get(/\/(.*)?.*/, restify.plugins.serveStatic({
    directory: './static',
    default: 'index.html'
}));
// Create chat connector for communicating with the Bot Framework Service
var connector = new builder.ChatConnector({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword,
    openIdMetadata: process.env.BotOpenIdMetadata
});
// Listen for messages from users 
server.post('/api/messages', connector.listen());
//setup botstate and main dialog
var bot = new builder.UniversalBot(connector);
bot.set('storage', statedata.getAzureBotStorage());
// Setup LUIS Model and Intent Dialogs
var model = process.env.LUIS_MODEL_URL;
var recognizer = new builder.LuisRecognizer(model);
var intents = new builder.IntentDialog({ recognizers: [recognizer] });
bot.dialog('/', intents);
// Welcome message
bot.on('conversationUpdate', function (session) {
    if (session.membersAdded) {
        session.membersAdded.forEach((identity) => {
            if (identity.id === session.address.bot.id) {
                bot.beginDialog(session.address, 'Welcome');
            }
        });
    }
});
bot.dialog('Welcome', [
    function (session) {
        session.send("Hey! I'm Ringo, the music bot 😎🎧🎵\r\nI love to discover new music and share my discoveries. "
            + "Tell me about Artists and Bands that you like, for example:\r\n"
            + "`I like Metallica`.\r\n"
            + "And you can type `help` or `quit` at any time.");
        //builder.Prompts.text(session, "What's your name?");
        session.endDialog();
    }
    /*
    ,
    function (session, results) {
        session.userData.username = results.response;
        session.send(`'Sup ${results.response}! I love to discover new music and share my discoveries ;) `);
        //+ "I'm not really very smart so you may have to be patient with me :) If I start bugging out, just type 'help'.");
        builder.Prompts.text(session, "Who are your favourite artists and bands?");
        session.endDialog();
},*/
]);
intents.matches('Help', [
    function (session, args, next) {
        session.endDialog("Ringo is a bot that aims to discover music by asking you a series of questions about your music tastes."
            + "You can type 'quit' at any time to quit and resume the conversation later.");
    }
]);
intents.matches('Quit', function (session, args, next) {
    session.endDialog("OK - see ya!");
});
intents.onDefault([
    function (session, args) {
        // if in a group and the bot is not mentioned, ignore this dialog
        if (helpers.isGroup(session.message) && !helpers.isMentioned(session.message)) {
            console.log('DEBUG: Ignoring Group conversation without bot mentioned');
            return;
        }
        ;
        _messages.sorry(session);
        session.endDialog();
    }
]);
intents.matches('Like Artist', (session, args, next) => __awaiter(this, void 0, void 0, function* () {
    //TODO: Channel handlers for Slack, Web, Teams, Skype, etc
    // Session logging
    //TODO: #340 Switch off last session logging
    session.userData.lastSessionMessage = session.message;
    session.userData.lastArgs = args;
    // if in a group and the bot is not mentioned, ignore this dialog
    if (helpers.isGroup(session.message) && !helpers.isMentioned(session.message)) {
        console.log('DEBUG: Ignoring Group conversation without bot mentioned');
        return;
    }
    ;
    if (args.entities == null) {
        session.send('LUIS unable to detect entity');
    }
    else {
        var spotifyUri = builder.EntityRecognizer.findEntity(args.entities, 'spotifyUri');
        if (spotifyUri) {
            // I like spotify:artist:25IG9fa7cbdmCIy3OnuH57
            // extract the entity with case preserved
            let uri = helpers.getEntityText(session.message, spotifyUri);
            session.sendTyping();
            try {
                let artist = yield _artists.getArtistByUri(uri);
                let msg = _messages.artist(session, artist);
                msg.text(`I like ${artist.name} too!`);
                session.send(msg);
                session.sendTyping();
                yield userdata.userLikesArtist(session.message.user.id, artist.name);
                let msg2 = yield _messages.recommendArtist(session, artist.id);
                session.endDialog(msg2);
            }
            catch (e) {
                _messages.whoops(session, e);
                return;
            }
            /*
            // if slack just send a URL
            if (session.message.source === 'slack' && result.match.artistSpotifyUrl){
                session.send(result.match.artistSpotifyUrl)
            } else {
                let audioCard = await cards.getAudioCard(session, artist);
                session.send(audioCard);
            }
            */
        }
        else {
            // I like Radiohead
            var artistsName = builder.EntityRecognizer.findEntity(args.entities, 'Music.ArtistName');
            if (!artistsName) {
                _messages.sorry(session);
                session.endDialog();
                return;
            }
            // 1. lookup the artist
            try {
                session.sendTyping();
                let artists = yield _artists.searchArtists(artistsName.entity, 3);
                let match = helpers.findMatch(artists);
                if (match.matched) {
                    // exact match
                    let msg = _messages.artist(session, match.artist);
                    msg.text(`I like ${match.artist.name} too!`);
                    session.send(msg);
                    session.sendTyping();
                    yield userdata.userLikesArtist(session.message.user.id, match.artist.name);
                    let msg2 = yield _messages.recommendArtist(session, match.artist.id);
                    session.endDialog(msg2);
                }
                else {
                    // Which artist?
                    let cards = _cards.artists(session, artists);
                    let msg = new builder.Message(session);
                    msg.attachmentLayout(builder.AttachmentLayout.carousel);
                    msg.attachments(cards);
                    msg.text(`Which ${artistsName.entity}?`);
                    session.send(msg);
                    session.endDialog();
                    return;
                }
            }
            catch (e) {
                _messages.whoops(session, e);
                return;
            }
        }
    }
}));
