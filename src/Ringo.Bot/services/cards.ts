import builder = require('botbuilder');
import _artist = require('../models/artist');

/**
 * Creates a simple Artist Hero Card
 * @param session 
 * @param artist 
 */
export function artist(session, artist: _artist.Artist, likeButton: boolean = false): builder.AttachmentType {
    let card = new builder.HeroCard(session)
        .title(artist.name);

    if (artist.images.length > 0) {
        // images 600 wide
        card.images([builder.CardImage.create(session, artist.images[0].url)]);
    }

    let buttons = [];
    if (likeButton){
        buttons.push(builder.CardAction.imBack(session, `I like ${artist.spotify.uri}`, `❤️ I like ${artist.name}`));
    }

    buttons.push(builder.CardAction.imBack(session, `Play ${artist.spotify.uri}`, `🎵 Play ${artist.name}`));
    card.buttons(buttons);

    return card;
};


export function artists(session, artists: _artist.Artist[]): builder.AttachmentType[] {
    let cards = new Array<builder.AttachmentType>();

    for (var j in artists) {

        let item = artists[j];

        // skip items with no images
        if (item.images.length === 0) continue;

        let card = new builder.HeroCard(session)
            .title(item.name)
            // images 600 wide
            .images([builder.CardImage.create(session, item.images[0].url)])
            .text(`Do you like ${item.name}?`)
            .buttons([
                builder.CardAction.imBack(session, `I like ${item.spotify.uri}`, `❤️ I like ${item.name}`),
                builder.CardAction.imBack(session, `Play ${item.spotify.uri}`, `🎵 Play ${item.name}`)
            ]);

        cards.push(card);
    }

    return cards;
};



/*
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
*/