"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const builder = require("botbuilder");
const _cards = require("./cards");
const _artists = require("./artists");
const _metrics = require("./metrics");
const _userdata = require("./userdata");
const user = require("../models/user");
function artist(session, artist) {
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
exports.artist = artist;
function popularityDesc(a, b) {
    if (a.popularity > b.popularity)
        return -1;
    if (a.popularity < b.popularity)
        return 1;
    return 0;
}
function recommendArtist(session, artistId) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            let artists = yield _artists.getRelatedArtists(artistId);
            if (artists.length === 0)
                return null;
            // filter out artists without images and sort by popularity desc
            let sorted = artists.filter(a => a.images.length > 0).sort(popularityDesc);
            // get the artists that are already liked
            let likedArtists = yield _userdata.artistsUserLikes(user.userId(session));
            // go through the list until one that has not been list is found, or return null
            var artist = null;
            for (var i = 0; i < sorted.length; i++) {
                if (likedArtists.find((l) => l.spotify.id === sorted[i].spotify.id)) {
                    console.log('messages.recommendArtist: skipped, already liked', sorted[i]);
                    continue;
                }
                // found one
                artist = sorted[i];
                break;
            }
            if (artist === null)
                return null;
            let card = _cards.artist(session, artist, true);
            let msg = new builder.Message(session);
            msg.attachments([card]);
            msg.text(`Do you like ${artist.name}?`);
            return msg;
        }
        catch (e) {
            whoops(session, e);
            throw e;
        }
    });
}
exports.recommendArtist = recommendArtist;
function sorry(session) {
    _metrics.trackEvent('Bot/Sorry');
    session.send('Sorry!! I didn\'t understand, try something like \'I like metallica \'');
}
exports.sorry = sorry;
function whoops(session, e) {
    _metrics.trackError(e);
    _metrics.trackEvent('Bot/Whoops');
    console.error(e);
    session.endDialog(`Whoops! Something is wrong 😞 Please try again`);
}
exports.whoops = whoops;
