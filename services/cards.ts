import _spotify = require('./spotify');
import builder = require('botbuilder');

export async function getArtists(session, artists: string[]) {
    let cards = new Array<builder.AttachmentType>();

    for (var i in artists) {
        let artist = artists[i];
        let artistData = await _spotify.searchArtists(artist, 1);
        if (artistData.artists.items.length > 0) {
            session.userData.faveArtist = artistData;
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
