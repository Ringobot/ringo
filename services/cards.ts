import _spotify = require('./spotify');
import builder = require('botbuilder');

function findMatch(items: any, result: any) {
    let itemsWithImages = items.filter(i => i.images.length > 0);
    result.matched = itemsWithImages.length === 1;
    if (result.matched) result.match = {
        artistName: itemsWithImages[0].name,
        artistId: itemsWithImages[0].id,
        artistUri: itemsWithImages[0].uri
    };
}

export async function getArtists(session, artists: string[]) {
    let cards = new Array<builder.AttachmentType>();
    let result = { msg: null, matched: false, match: { artistName: null, artistUid: null } };

    for (var i in artists) {
        let artist = artists[i];
        let artistData = await _spotify.searchArtists(artist, 3);
        if (artistData.artists.items.length > 0) {
            //session.userData.faveArtist = artistData; // ??

            findMatch(artistData.artists.items, result);

            for (var j in artistData.artists.items) {

                let item = artistData.artists.items[j];

                // skip items with no images
                if (item.images.length === 0) continue;

                let card = new builder.HeroCard(session)
                    .title(item.name)
                    // images 600 wide
                    .images([builder.CardImage.create(session, item.images[0].url)])

                if (!result.matched) {
                    card.text(`Do you like ${item.name}?`);
                    card.buttons([
                        builder.CardAction.imBack(session, `I like ${item.name}`, `I like ${item.name}`)
                    ]);
                }

                cards.push(card);
            }
        }
    }

    if (cards.length == 0) return result;

    let msg = new builder.Message(session);
    msg.attachmentLayout(builder.AttachmentLayout.carousel);
    msg.attachments(cards);
    result.msg = msg;
    return result;
};

export async function getRelatedArtists(session, artistId: string, limit: number = 1) {
    let cards = new Array<builder.AttachmentType>();

    let result = await _spotify.getRelatedArtists(artistId);
    if (result.artists.length == 0) return null;

    // filter out artists without images and sort by popularity desc
    let artists = result.artists.filter(a => a.images.length > 0).sort((a, b)=>{
        if (a.popularity > b.popularity) return -1;
        if (a.popularity < b.popularity) return 1;
        return 0;
    });

    for (var i = 0; i < limit; i++) {
        cards.push(
            new builder.HeroCard(session)
                .title(artists[i].name)
                .text(`Do you like ${artists[i].name}?`)
                // images 600 wide
                .images([builder.CardImage.create(session, artists[i].images[0].url)])
                .buttons([
                    builder.CardAction.imBack(session, `I like ${artists[i].name}`, `I like ${artists[i].name}`)
                ])
        );
    }

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
