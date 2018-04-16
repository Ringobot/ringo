import builder = require('botbuilder');
import _cards = require('./cards');
import _artists = require('./artists');

export function artist(session: builder.Session, artist: any): builder.Message {
    let card = _cards.artist(session, artist);
    let msg = new builder.Message(session);
    //msg.attachmentLayout(builder.AttachmentLayout.carousel);
    msg.attachments([card]);

    /*
    msg.suggestedActions(
        builder.SuggestedActions.create(
            session, [
                builder.CardAction.imBack(session, `Play ${artist.uri}`, `Play ${artist.name} now`)
            ])
    );
    */

    return msg;
}

function popularityDesc(a, b){
    if (a.popularity > b.popularity) return -1;
    if (a.popularity < b.popularity) return 1;
    return 0;
}

export async function recommendArtist(session: builder.Session, artistId: string) : Promise<builder.Message> {
    try {
        let artists = await _artists.getRelatedArtists(artistId);
        if (artists.length === 0) return null;

        // filter out artists without images and sort by popularity desc
        let sorted = artists.artists.filter(a => a.images.length > 0).sort(popularityDesc);
        let artist = sorted[0];
        let card = _cards.artist(session, artist, true);

        let msg = new builder.Message(session);
        //msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments([card]);
        msg.text(`Do you like ${artist.name}?`);
        return msg;

    } catch (e) {
        whoops(session, e);
        throw e;
    }
}

export function sorry(session) {
    session.send('Sorry!! I didn\'t understand, try something like \'I like metallica \'');
}

export function whoops(session, e) {
    console.error(e);
    session.endDialog(`Whoops! Something is wrong 😞 Please try again`);
}
