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

// main dialog
var bot = new builder.UniversalBot(connector, [
    function (session) {
        session.send("Hey! I'm Ringo, the music bot 😎🎧🎵");
        builder.Prompts.text(session, "What's your name?");
    },
    function (session, results) {
        session.userData.username = results.response;
        session.send(`'Sup ${results.response}! I love to discover new music and share my discoveries ;) `);
        //+ "I'm not really very smart so you may have to be patient with me :) If I start bugging out, just type 'help'.");
        session.beginDialog('fave_artists');
    },
    function (session, results) {
        session.endDialog();
    }]
).set('storage', statedata.getAzureBotStorage()); // use Table Storage for state data

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
}).triggerAction({
    matches: /^cancel$|^goodbye$|^quit$|^end$/i,
});

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
                session.beginDialog('fave_artists'); //TODO IS this correct?
            }
        }
        catch (e) {
            console.error(e);
            session.send(`Whoops! Something is wrong 😞 Please try again.`);
            session.beginDialog('fave_artists');
        }
    }
]);

bot.dialog('like_artist', [function (session, results) {
    // get the raw input
    let response = results.intent.matched.input;
    // strip the match, leaving the artist
    let artist = response.replace(/i\s(\w+\s)*(love|like)\s/i, '');
    session.sendTyping();
    
    try {
        // save the like
        userdata.userLikesArtist(session.userData.username, artist);
        session.endDialog(`I like ${artist} too!`);
    }
    catch (e) {
        console.error(e);
        session.send(`Whoops! Something is wrong 😞 Please try again.`);
        session.beginDialog('fave_artists');
    }

}]).triggerAction({
    // https://regex101.com/r/VO2I8r/1
    // 'I really like Radiohead'
    matches: /i\s(\w+\s)*(love|like)\s/i
});