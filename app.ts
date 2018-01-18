/// <reference types="node" />

require('dotenv').config();
import restify = require('restify');
import builder = require('botbuilder');
import cards = require('./services/cards');
import userdata = require('./services/userdata');
import statedata = require('./services/statedata');

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
        function (session){
            session.send("Hey! I'm Ringo, the music bot 😎🎧🎵");
            builder.Prompts.text(session, "What's your name?");
        },
        function (session, results) {
            session.userData.username = results.response;
            session.send(`'Sup ${results.response}! I love to discover new music and share my discoveries ;) `);
            //+ "I'm not really very smart so you may have to be patient with me :) If I start bugging out, just type 'help'.");
            builder.Prompts.text(session, "Who are your favourite artists and bands?");
            session.endDialog();
    },
]);

intents.matches('Help', [ 
    function (session, args, next) {
            session.endDialog("Ringo is a bot that aims to discover music by asking you a series of questions about your music tastes."
                + "You can type 'quit' at any time to quit and resume the conversation later.");
    }
]);

intents.matches('Quit', 
    function (session, args, next) {
        session.endDialog("OK - see ya!");
    }
);

intents.onDefault([
    function(session){
        session.send('Sorry!! I didn\'t understand, try something like \'I like metallica \'' );
        session.endDialog();
	}
]);

intents.matches('Artist', 
        async (session, args, next) => {
            if (args.entities == null) {
                session.send('LUIS unable to detect entity');
            }
            else {
                var artistsName = [];
                args.entities.forEach(ent => {
                    //var artistsEntity = builder.EntityRecognizer.findEntity(e, 'ArtistNameSimple');
                    artistsName.push(ent.entity);
                });
                session.sendTyping();
                try {
                    let msg = await cards.getArtists(session, artistsName);
                    if (msg) {
                        session.send(msg);
                        session.endDialog();
                    }
                    else {
                        session.send(`Sorry I couldn't find anything for "${artistsName}" 😞 Try something like, "Metallica, Ed Sheeran"`);
                        session.endDialog();
                    }
                }
                catch (e) {
                    console.error(e);
                    session.send(`Whoops! Something is wrong 😞 in the Artist diaglog, please try again.`);
                }
            };
    }
);

intents.matches('Like_Artist', 
        async (session, args, next) => {
            if (args.entities == null) {
                session.send('LUIS unable to detect entity');
            }
            else {

                var artistsName = builder.EntityRecognizer.findEntity(args.entities, 'ArtistNameSimple');
                session.sendTyping();
                try {
                    // save the like
                    userdata.userLikesArtist(session.userData.username, artistsName.entity);
                    session.send(`I like ${artistsName.entity} too!`);
                    session.send(`Here are some recommended albums and tracks based on your intrests`);
                    var aritistSeed = session.userData.faveArtist.artists.items[0].id;
                    let msg = await cards.getRecommendations(session, aritistSeed);
                    if (msg) {
                        session.send(msg);
                        session.endDialog();
                    }
                }
                catch (e) {
                    console.error(e);
                    session.send(`Whoops! Something is wrong 😞 in the Like_Artist diaglog, please try again.`);
                }
            };
    }
);
