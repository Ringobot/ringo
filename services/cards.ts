import _spotify = require('./spotify');
import builder = require('botbuilder');

export async function getArtists(session, artists: string[]) {
    let cards = new Array<builder.AttachmentType>();
    let result = { msg: null, oneResult: false };

    for (var i in artists) {
        let artist = artists[i];
        let artistData = await _spotify.searchArtists(artist, 3);
        if (artistData.artists.items.length > 0) {
            session.userData.faveArtist = artistData; // ??

            result.oneResult = artistData.artists.items.filter(i=> i.images.length > 0).length === 1;

            for (var j in artistData.artists.items) {

                let item = artistData.artists.items[j];

                // skip items with no images
                if (item.images.length === 0) continue;

                let card = new builder.HeroCard(session)
                    .title(item.name)
                    // images 600 wide
                    .images([builder.CardImage.create(session, item.images[0].url)])

                if (!result.oneResult) {
                    card.text(`Do you like ${item.name}?`);
                    card.buttons([
                        builder.CardAction.imBack(session, `I like ${item.name}`, `I like ${item.name}`)
                    ]);
                }

                cards.push(card);
            }
        }
    }

    if (cards.length == 0) return;

    let msg = new builder.Message(session);
    msg.attachmentLayout(builder.AttachmentLayout.carousel);
    msg.attachments(cards);
    result.msg = msg;
    return result;
};

export async function getRecommendations(session, artistSeed: string) {
    let cards = new Array<builder.AttachmentType>();

    let recommendationData = await _spotify.getRecommendation(artistSeed, 3);
    if (recommendationData.tracks.length > 0) {

        //let data = new Array(recommendationData.tracks);
        //let album = data.filter(item => item.

        recommendationData.tracks.forEach(album => {
            cards.push(
                new builder.HeroCard(session)
                    .title(album.name)
                    .text(`Do you like ${album.name} by ${album.artists[0].name}?`)
                    // images 600 wide
                    .images([builder.CardImage.create(session, album.album.images[0].url)])
                    .buttons([
                        builder.CardAction.imBack(session, `I like album ${album.name}`, `I like album ${album.name}`)
                    ])
            );
        });
    };

    if (cards.length == 0) return;

    let msg = new builder.Message(session);
    msg.attachmentLayout(builder.AttachmentLayout.carousel);
    msg.attachments(cards);
    return msg;
};
