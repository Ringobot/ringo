/// <reference types="node" />

require('dotenv').config();
import _metrics = require('./services/metrics');
_metrics.start();

import restify = require('restify');
import builder = require('botbuilder');
import _artists = require('./services/artists');
import _cards = require('./services/cards');
import _messages = require('./services/messages');
import userdata = require('./services/userdata');
import statedata = require('./services/statedata');
import servicebus = require('./services/servicebus')
import helpers = require('./helpers');
import _spotify = require('./services/spotify');
import _spotifyAuth = require('./services/spotifyauth');
import user = require('./models/user')

// Setup Restify Server
var server = restify.createServer();
server.use(restify.plugins.queryParser());

server.listen(process.env.port || process.env.PORT || 3978, function (e) {
    if (e) throw e;
    console.log('%s listening to %s', server.name, server.url);
});

// Create chat connector for communicating with the Bot Framework Service
var connector = new builder.ChatConnector({
    appId: process.env.MicrosoftAppId,
    appPassword: process.env.MicrosoftAppPassword,
    openIdMetadata: process.env.BotOpenIdMetadata
});

// Listen for messages from users 
server.post('/api/messages', connector.listen());
server.get('/authorize/spotify', _spotifyAuth.authorizeCallback);
server.get('/authorize/spotify/:userHash', _spotifyAuth.authorize);

//setup botstate and main dialog
var bot = new builder.UniversalBot(connector);
bot.set('storage', statedata.getAzureBotStorage());

// Setup LUIS Model and Intent Dialogs
var model = process.env.LUIS_MODEL_URL;
var recognizer = new builder.LuisRecognizer(model);

var intents = new builder.IntentDialog({ recognizers: [recognizer] });
bot.dialog('/', intents);

let userHash = (session:any) :string | null => {
    let userId = ((session && session.user && session.user.id) 
        || (session && session.message && session.message.user && session.message.user.id));
    if (!userId) {
        console.warn('Could not find user Id in Session');
        return null;
    }
    return user.userHash(userId);
}

function sessionId (session) {
    //session.address.conversation.id
    //session.message.address.conversation.id
    let sessionId = ((session && session.message && session.message.address 
        && session.message.address.conversation && session.message.address.conversation.id))
        || (session && session.address && session.address.conversation && session.address.conversation.id);
        
    if (!sessionId) {
        console.warn('Could not find session Id in Session');
        return null;
    }
    return sessionId;
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
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
        _metrics.trackEvent('Bot/Welcome');

        session.send(
            "Hey! I'm Ringo, the music bot 😎🎧🎵\r\nI love to discover new music and share my discoveries. "
            + "Tell me about Artists and Bands that you like, for example:\r\n"
            + "`I like Metallica`.\r\n"
            + "And you can type `help` or `quit` at any time."
        );

        session.endDialog();
    }
]);

intents.matches('Help', [
    function (session, args, next) {
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
        _metrics.trackEvent('Bot/Help');

        session.endDialog("Ringo is a bot that aims to discover music by asking you a series of questions about your music tastes."
            + "You can type 'quit' at any time to quit and resume the conversation later.");
    }
]);

intents.matches('Quit',
    function (session, args, next) {
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));
        _metrics.trackEvent('Bot/Quit');

        session.endDialog("OK - see ya!");
    }
);

intents.onDefault([
    function (session, args) {
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));

        // if in a group and the bot is not mentioned, ignore this dialog
        if (helpers.isGroup(session.message) && !helpers.isMentioned(session.message)) {
            console.log('DEBUG: Ignoring Group conversation without bot mentioned');
            return
        };

        _messages.sorry(session);
        session.endDialog();
    }
]);


intents.matches('Like Artist',
    async (session, args, next) => {
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));

        // Session logging
        //TODO: #340 Switch off last session logging
        session.userData.lastSessionMessage = session.message;
        session.userData.lastArgs = args;

        // if in a group and the bot is not mentioned, ignore this dialog
        if (helpers.isGroup(session.message) && !helpers.isMentioned(session.message)) {
            console.log('DEBUG: Ignoring Group conversation without bot mentioned');
            return
        };

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
                let artist = await _artists.getArtistByUri(uri);
                //let msg = _messages.artist(session, artist);
                //msg.text(`I like ${artist.name} too!`);
                //session.send(msg);
                session.send(`I like ${artist.name} too!`);
                _metrics.trackEvent('Artist/Like');
                session.sendTyping();
                await userdata.userLikesArtist(session.message.user.id, artist.name);

                let msg2 = await _messages.recommendArtist(session, artist.id);
                session.endDialog(msg2);

            } catch (e) {
                _messages.whoops(session, e);
                return;
            }

        } else {
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
                let artists = await _artists.searchArtists(artistsName.entity, 3);
                let match = helpers.findMatch(artists);

                if (match.matched) {
                    // exact match
                    let msg = _messages.artist(session, match.artist);
                    msg.text(`I like ${match.artist.name} too!`);
                    session.send(msg);
                    _metrics.trackEvent('Artist/Like');

                    session.sendTyping();
                    await userdata.userLikesArtist(session.message.user.id, match.artist.name);

                    let msg2 = await _messages.recommendArtist(session, match.artist.id);
                    session.endDialog(msg2);

                } else {
                    // Which artist?
                    let cards = _cards.artists(session, artists);
                    let msg = new builder.Message(session);
                    msg.attachmentLayout(builder.AttachmentLayout.carousel);
                    msg.attachments(cards);
                    msg.text(`Which ${artistsName.entity}?`);

                    session.send(msg)
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
    }
);

intents.matches('Play',
    async (session, args, next) => {
        _metrics.setAuthenticatedUserContext(sessionId(session), userHash(session));

        // Session logging
        //TODO: #340 Switch off last session logging
        session.userData.lastSessionMessage = session.message;
        session.userData.lastArgs = args;

        // if in a group and the bot is not mentioned, ignore this dialog
        if (helpers.isGroup(session.message) && !helpers.isMentioned(session.message)) {
            console.log('DEBUG: Ignoring Group conversation without bot mentioned');
            return
        };

        if (args.entities == null) {
            _messages.sorry(session);
            return;
        }

        // 1. get spotify artist URI entity
        let spotifyUriEntity = builder.EntityRecognizer.findEntity(args.entities, 'spotifyUri');
        var spotifyUri = null;

        if (spotifyUriEntity){
            spotifyUri = helpers.getEntityText(session.message, spotifyUriEntity);
        } else {
            // Play Radiohead
            var artistsName = builder.EntityRecognizer.findEntity(args.entities, 'Music.ArtistName');

            if (artistsName) {

                // Lookup the artist
                try {
                    session.sendTyping();
                    let artists = await _artists.searchArtists(artistsName.entity, 3);
                    let match = helpers.findMatch(artists);

                    if (match.matched) {
                        // exact match
                        let msg = _messages.artist(session, match.artist);
                        spotifyUri = match.artist.uri;
                    } else {
                        // Which artist?
                        let cards = _cards.artists(session, artists);
                        let msg = new builder.Message(session);
                        msg.attachmentLayout(builder.AttachmentLayout.carousel);
                        msg.attachments(cards);
                        msg.text(`Which ${artistsName.entity}?`);

                        session.send(msg)
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
            await _spotify.playArtist(session.message.user.id, spotifyUri);

        } catch (e) {
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
    }
);
