import builder = require('botbuilder');
import _cards = require('./cards');
import _artists = require('./artists');
import _artist = require('../models/artist');
import _metrics = require('./metrics');
import _userdata = require('./userdata');
import user = require('../models/user');


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
        let artists:_artist.Artist[] = await _artists.getRelatedArtists(artistId);
        if (artists.length === 0) return null;

        // filter out artists without images and sort by popularity desc
        let sorted = artists.filter(a => a.images.length > 0).sort(popularityDesc);

        // get the artists that are already liked
        let likedArtists = await _userdata.artistsUserLikes(user.userId(session));

        // go through the list until one that has not been list is found, or return null
        var artist = null;
        for (var i = 0; i < sorted.length; i++){
            if (likedArtists.find((l)=> l.spotify.id === sorted[i].spotify.id)){
                console.log('messages.recommendArtist: skipped, already liked', sorted[i])
                continue
            }

            // found one
            artist = sorted[i];
            break
        }

        if (artist === null) return null;

        let card = _cards.artist(session, artist, true);

        let msg = new builder.Message(session);
        msg.attachments([card]);
        msg.text(`Do you like ${artist.name}?`);
        return msg;

    } catch (e) {
        whoops(session, e);
        throw e;
    }
}

export function sorry(session) {
    _metrics.trackEvent('Bot/Sorry');
    session.send('Sorry!! I didn\'t understand, try something like \'I like metallica \'');
}

export function whoops(session, e) {
    _metrics.trackError(e);
    _metrics.trackEvent('Bot/Whoops');
    console.error(e);
    session.endDialog(`Whoops! Something is wrong 😞 Please try again`);
}
