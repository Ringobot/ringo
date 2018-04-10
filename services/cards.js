"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const builder = require("botbuilder");
/*
export function getAudioCard(session: builder.Session, artist:any, track:any): builder.AttachmentType {
    if (!(track && track.name && track.preview_url)) throw "Expecting track {name:string, preview_url:string}";

    let audioCard = new builder.AudioCard(session)
        .image(builder.CardImage.create(session, artist.images[0].url))
        .media([{url:track.preview_url, profile:""}])
        .title(artist.name)
        .subtitle(track.name)
        .shareable(false);

    cards.push(audioCard);

    let msg = new builder.Message(session);
    msg.attachmentLayout(builder.AttachmentLayout.carousel);
    msg.attachments(cards);
    return msg;
}
*/
/**
 * Creates a simple Artist Hero Card
 * @param session
 * @param artist
 */
function artist(session, artist) {
    let card = new builder.HeroCard(session)
        .title(artist.name);
    if (artist.images.length > 0) {
        // images 600 wide
        card.images([builder.CardImage.create(session, artist.images[0].url)]);
    }
    /*
    card.buttons([
        builder.CardAction.imBack(session, `I like ${artist.uri}`, `I like ${artist.name}`)
    ]);
    */
    return card;
}
exports.artist = artist;
;
function artists(session, artists) {
    let cards = new Array();
    for (var j in artists.artists.items) {
        let item = artists.artists.items[j];
        // skip items with no images
        if (item.images.length === 0)
            continue;
        let card = new builder.HeroCard(session)
            .title(item.name)
            .images([builder.CardImage.create(session, item.images[0].url)])
            .text(`Do you like ${item.name}?`)
            .buttons([
            builder.CardAction.imBack(session, `I like ${item.uri}`, `I like ${item.name} ❤️`),
            builder.CardAction.imBack(session, `Play ${item.uri}`, `Play ${item.name} now 🎵`)
        ]);
        cards.push(card);
    }
    return cards;
}
exports.artists = artists;
;
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
