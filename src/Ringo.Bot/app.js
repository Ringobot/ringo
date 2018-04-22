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
/// <reference types="node" />
require('dotenv').config();
const _metrics = require("./services/metrics");
_metrics.start();
const restify = require("restify");
const builder = require("botbuilder");
const _artists = require("./services/artists");
const _cards = require("./services/cards");
const _messages = require("./services/messages");
const userdata = require("./services/userdata");
const statedata = require("./services/statedata");
const helpers = require("./helpers");
const _spotify = require("./services/spotify");
const _spotifyAuth = require("./services/spotifyauth");
const user = require("./models/user");
const _crypto = require("./helpers/crypto");
// Setup Restify Server
var server = restify.createServer();
server.use(restify.plugins.queryParser());
server.listen(process.env.port || process.env.PORT || 3978, function (e) {
    if (e)
        throw e;
    console.log('%s listening to %s', server.name, server.url);
});
// Create chat connector for communicating with the Bot Framework Service
var connector = new builder.ChatConnector({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword,
    openIdMetadata: process.env.BotOpenIdMetadata
});
server.post('/api/messages', connector.listen());
server.get('/authorize/spotify', _spotifyAuth.authorizeCallback);
server.get('/authorize/spotify/:userHash', _spotifyAuth.authorize);
// last route for static files
server.get(/\/(.*)?.*/, restify.plugins.serveStatic({
    directory: './static',
    default: 'index.html'
}));
//setup botstate and main dialog
var bot = new builder.UniversalBot(connector);
bot.set('storage', statedata.getAzureBotStorage());
/*
const intentDialog = new builder.IntentDialog({
    recognizers: [expressionRecognizer, defaultRecognizer, recognizer],
    recognizeOrder: 'series'
});
*/
// Install a custom recognizer to look for user saying 'help' or 'goodbye'.
let custom = {
    recognize: function (context, done) {
        if (context.message && notListening(context.message)) {
            // ignore non group/DM messages so that they don't go to LUIS
            done(null, { score: 1.0, intent: 'Ignore' });
            return;
        }
        var intent = { score: 0.0, intent: null };
        if (context.message && context.message.text) {
            let mention = (context.message.entities
                && context.message.entities.filter(e => e.type == 'mention')
                    .find(e => e.mentioned.id == context.message.address.bot.id));
            // filter out mention handle
            let message = mention ? context.message.text.replace(mention.text, '').trim() : context.message.text;
            switch (message.toLowerCase()) {
                case 'help':
                    intent = { score: 1.0, intent: 'Help' };
                    break;
                case 'quit':
                case 'goodbye':
                case 'exit':
                    intent = { score: 1.0, intent: 'Quit' };
                    break;
            }
            // Feedback
            if ((/^feedback.*/i).test(message)) {
                intent = { score: 1.0, intent: 'Feedback' };
            }
        }
        done(null, intent);
    }
};
// switch recognizer
let recognizers = [
    custom,
    //new builder.RegExpRecognizer( "FeedbackIntent", { en_us: /^\/(feedback)/i}),
    new builder.LuisRecognizer(process.env.LUIS_MODEL_URL)
];
// serialise the recognizers so that the custom recognizer is called before LUIS (saving on LUIS calls)
var intents = new builder.IntentDialog({ recognizers: recognizers, recognizeOrder: builder.RecognizeOrder.series });
bot.dialog('/', intents);
let userHash = (session) => {
    let userId = ((session && session.user && session.user.id)
        || (session && session.message && session.message.user && session.message.user.id));
    if (!userId) {
        console.warn('Could not find user Id in Session');
        return null;
    }
    return user.userHash(userId);
};
function sessionId(session) {
    let sessionId = ((session && session.message && session.message.address
        && session.message.address.conversation && session.message.address.conversation.id))
        || (session && session.address && session.address.conversation && session.address.conversation.id);
    if (!sessionId) {
        console.warn('Could not find session Id in Session');
        return null;
    }
    return sessionId;
}
function sessionHash(session) {
    let id = sessionId(session);
    return id ? _crypto.sha256(id) : null;
}
// Welcome message
bot.on('conversationUpdate', function (session) {
    _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
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
        if (notListening(session.message))
            return;
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
        _metrics.trackEvent('Bot/Welcome');
        session.send("Hey! I'm Ringo, the music bot 😎🎧🎵\r\nI love to discover new music and share my discoveries. "
            + "Tell me about Artists and Bands that you like, for example:\r\n"
            + "`I like Metallica`.\r\n"
            + "And you can type `help`, `quit` or `feedback` at any time.");
        session.endDialog();
    }
]);
intents.matches('Feedback', [
    function (session) {
        if (notListening(session.message))
            return;
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
        _metrics.trackEvent('Bot/Feedback');
        let message = session.message.text;
        builder.Prompts.text(session, 'How can I improve? If you would like to be contacted, include your email address in your feedback');
    },
    function (session, results) {
        session.endDialog('Thanks! I have sent your feedback.');
        let message = results.response;
    }
]);
intents.matches('Ignore', function () {
    console.log('Not listening');
});
intents.matches('Help', function (session) {
    if (notListening(session.message))
        return;
    _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
    _metrics.trackEvent('Bot/Help');
    session.endDialog("Ringo is a bot that aims to discover music by asking you a series of questions about your music tastes."
        + "You can type `quit` at any time to quit and resume the conversation later. Or type `feedback` to send feedback");
});
intents.matches('Quit', function (session) {
    if (notListening(session.message))
        return;
    _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
    _metrics.trackEvent('Bot/Quit');
    session.endDialog("OK - see ya!");
});
function notListening(message) {
    // if in a group and the bot is not mentioned, ignore this dialog
    return (helpers.isGroup(message) && !helpers.isMentioned(message));
}
intents.onDefault([
    function (session, args) {
        if (notListening(session.message))
            return;
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
        _messages.sorry(session);
        session.endDialog();
    }
]);
intents.matches('Like Artist', (session, args, next) => __awaiter(this, void 0, void 0, function* () {
    if (notListening(session.message))
        return;
    _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
    console.log(`Like: intent = ${args.intent}, score = ${args.score}, entities = ${args.entities.length}, message = "${session.message.text}"`);
    // 90% certainty to like
    if (args.score < 0.9) {
        _messages.sorry(session);
        return;
    }
    // Session logging
    //TODO: #340 Switch off last session logging
    session.userData.lastSessionMessage = session.message;
    session.userData.lastArgs = args;
    if (args.entities == null) {
        _messages.sorry(session);
        return;
    }
    var spotifyUri = builder.EntityRecognizer.findEntity(args.entities, 'spotifyUri');
    if (spotifyUri) {
        // I like spotify:artist:25IG9fa7cbdmCIy3OnuH57
        // extract the entity with case preserved
        let uri = helpers.getEntityText(session.message, spotifyUri);
        session.sendTyping();
        try {
            let artist = yield _artists.getArtistByUri(uri);
            //let msg = _messages.artist(session, artist);
            //msg.text(`I like ${artist.name} too!`);
            //session.send(msg);
            session.send(`I like ${artist.name} too!`);
            _metrics.trackEvent('Artist/Like');
            session.sendTyping();
            yield userdata.userLikesArtist(session.message.user.id, artist.name);
            let msg2 = yield _messages.recommendArtist(session, artist.id);
            session.endDialog(msg2);
        }
        catch (e) {
            _messages.whoops(session, e);
            return;
        }
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
                _metrics.trackEvent('Artist/Like');
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
                _metrics.trackEvent('Artist/LikeWhichArtist');
                return;
            }
        }
        catch (e) {
            _messages.whoops(session, e);
            return;
        }
    }
}));
intents.matches('Play', (session, args) => __awaiter(this, void 0, void 0, function* () {
    if (notListening(session.message))
        return;
    _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
    console.log(`Play: intent = ${args.intent}, score = ${args.score}, entities = ${args.entities.length}, message = "${session.message.text}"`);
    // 90% certainty to play
    if (args.score < 0.9) {
        _messages.sorry(session);
        return;
    }
    // Session logging
    //TODO: #340 Switch off last session logging
    session.userData.lastSessionMessage = session.message;
    session.userData.lastArgs = args;
    if (args.entities == null) {
        _messages.sorry(session);
        return;
    }
    // 1. get spotify artist URI entity
    let spotifyUriEntity = builder.EntityRecognizer.findEntity(args.entities, 'spotifyUri');
    var spotifyUri = null;
    if (spotifyUriEntity) {
        spotifyUri = helpers.getEntityText(session.message, spotifyUriEntity);
    }
    else {
        // Play Radiohead
        var artistsName = builder.EntityRecognizer.findEntity(args.entities, 'Music.ArtistName');
        if (artistsName) {
            // Lookup the artist
            try {
                session.sendTyping();
                let artists = yield _artists.searchArtists(artistsName.entity, 3);
                let match = helpers.findMatch(artists);
                if (match.matched) {
                    // exact match
                    let msg = _messages.artist(session, match.artist);
                    spotifyUri = match.artist.uri;
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
                    _metrics.trackEvent('Artist/PlayWhichArtist');
                    return;
                }
            }
            catch (e) {
                _messages.whoops(session, e);
                return;
            }
        }
    }
    // Play artist
    try {
        _metrics.trackEvent('Artist/Play');
        yield _spotify.playArtist(session.message.user.id, spotifyUri);
    }
    catch (e) {
        if (e == 'Not Authorised') {
            _metrics.trackEvent('User/NotAuthorized');
            // If not authorised post message and link
            session.endDialog("I would love to play this song for you. But first I need you to tell Spotify that it's OK. Click this link "
                + `to authorise Ringo to control Spotify: ${process.env.SpotifyAuthRedirectUri}/${user.userHash(session.message.user.id)}`);
            return;
        }
        if (e.search('Command failed: Not paused') > 0) {
            _metrics.trackEvent('Artist/PlayAlreadyPlaying');
            session.endDialog("Already playing ;) try something like `Play Madonna`");
            return;
        }
        _messages.whoops(session, e);
    }
}));
