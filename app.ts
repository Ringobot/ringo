/// <reference types="node" />


import restify = require('restify');
import builder = require('botbuilder');
import cards = require('./services/cards');

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
        session.send("Hey! I'm Ringo, the music bot from Rdostr :]");
        builder.Prompts.text(session, "What's your name?");
    },
    function (session, results) {
        session.dialogData.name = results.response;
        session.send(`'Sup ${session.dialogData.name}! I love to discover new music and share my discoveries ;) ` 
            + "I'm not really very smart so you may have to be patient with me :) If I start bugging out, just type 'help'.");
        builder.Prompts.text(session, "Who are your favourite artists and bands?");
    },
    function (session, results) {
        session.dialogData.artists = results.response.split(',');
        session.sendTyping();
        cards.getArtists(session, session.dialogData.artists, function(error, msg) {
            session.send(msg);
            session.send(`234 of my friends listen to Radiohead! What is your favourite Radiohead album?`);
            session.endDialog();                
        });
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

