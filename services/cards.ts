import _spotify = require('./spotify');
import builder = require('botbuilder');

export async function getArtists(session, artists: string[]) {
    let cards = new Array<builder.AttachmentType>();

    for (var i in artists) {
        let artist = artists[i];
        let artistData = await _spotify.searchArtists(artist, 1);
        if (artistData.artists.items.length > 0) {
            let item = artistData.artists.items[0];
            cards.push(
                new builder.HeroCard(session)
                    .title(item.name)
                    .text(`Do you like ${item.name}?`)
                    // images 600 wide
                    .images([builder.CardImage.create(session, item.images[0].url)])
                    .buttons([
                        builder.CardAction.imBack(session, `I like ${item.name}`, `I like ${item.name}`)
                    ])
            );
        }
    }

    if (cards.length == 0) return;

    let msg = new builder.Message(session);
    msg.attachmentLayout(builder.AttachmentLayout.carousel);
    msg.attachments(cards);
    return msg;
};
