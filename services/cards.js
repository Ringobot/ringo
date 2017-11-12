var _spotify = require('./spotify');
var builder = require('botbuilder');

module.exports.getArtists = function (session, artists, callback) {
    
    var msg = new builder.Message(session);
    msg.attachmentLayout(builder.AttachmentLayout.carousel)
    msg.attachments([
        new builder.HeroCard(session)
            .title("Radiohead")
            //.subtitle("Subtitle")
            .text("234 of my friends listen to Radiohead")
            // images 600 wide
            .images([builder.CardImage.create(session, 'https://i.scdn.co/image/afcd616e1ef2d2786f47b3b4a8a6aeea24a72adc')])
            .buttons([
                builder.CardAction.messageBack(session, "This is \"Radiohead\"", "Yep"),
                builder.CardAction.messageBack(session, "This is not \"Radiohead\"", "Nope")
            ]),
        new builder.HeroCard(session)
            .title("Massive Attack")
            //.subtitle("100% Soft and Luxurious Cotton")
            .text("2,011 of my friends listen to Massive Attack")
            .images([builder.CardImage.create(session, 'https://i.scdn.co/image/c8bbeedb05f38ae5cb982a7daf4bf7129cca892c')])
            .buttons([
                builder.CardAction.messageBack(session, "This is \"Massive Attack\"", "Yep"),
                builder.CardAction.messageBack(session, "This is not \"Massive Attack\"", "Nope")
            ])
    ]);
    
    callback(null, msg);
};
    