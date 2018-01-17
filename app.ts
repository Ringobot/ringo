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
            //session.beginDialog('fave_artists');
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
	}
]);

intents.matches('Artist', 
        async (session, args, next) => {
            if (args.entities == null) {
                session.send('LUIS unable to detect entity');
            }
            else {
            // Resolve and store any HomeAutomation.Device entity passed from LUIS.
                var artistsName = builder.EntityRecognizer.findEntity(args.entities, 'ArtistNameSimple');
                session.sendTyping();
                try {
                    let msg = await cards.getArtists(session, artistsName.entity);
                    if (msg) {
                        session.send(msg);
                        session.beginDialog('fave_artists');
                    }
                    else {
                        session.send(`Sorry I couldn't find anything for "${artistsName.entity}" 😞 Try something like, "I like Metallica"`);
                    }
                }
                catch (e) {
                    console.error(e);
                    session.send(`Whoops! Something is wrong 😞 Please try again.`);
                }
            };
    }
);

bot.dialog('fave_artists', [
    function (session, results) {
        builder.Prompts.text(session, "Who are your favourite artists and bands?");
    },
    async function (session, results) {
        var artists: string[] = results.response.split(',');
        session.dialogData.artists = artists;
        session.sendTyping();
        try {
            let msg = await cards.getArtists(session, artists);
            if (msg) session.send(msg);
            else {
                session.send(`I couldn't find anything for "${results.response}" 😞 Try using commas, like "Lorde, Taylor Swift"`);
                //session.beginDialog('fave_artists'); //TODO IS this correct?
            }
        }
        catch (e) {
            console.error(e);
            session.send(`Whoops! Something is wrong 😞 Please try again.`);
            //session.beginDialog('fave_artists');
        }
    }
]);