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
            let sorted = artists.artists.filter(a => a.images.length > 0).sort(popularityDesc);
            let artist = sorted[0];
            let card = _cards.artist(session, artist, true);
            let msg = new builder.Message(session);
            //msg.attachmentLayout(builder.AttachmentLayout.carousel);
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
    session.send('Sorry!! I didn\'t understand, try something like \'I like metallica \'');
}
exports.sorry = sorry;
function whoops(session, e) {
    console.error(e);
    session.endDialog(`Whoops! Something is wrong 😞 Please try again`);
}
exports.whoops = whoops;
