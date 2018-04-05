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
const _spotify = require("./spotify");
const builder = require("botbuilder");
function findMatch(items, result) {
    let itemsWithImages = items.filter(i => i.images.length > 0);
    result.matched = itemsWithImages.length === 1;
    if (result.matched)
        result.match = {
            artistName: itemsWithImages[0].name,
            artistId: itemsWithImages[0].id,
            artistUri: itemsWithImages[0].uri
        };
}
function getArtistByUri(session, uri) {
    return __awaiter(this, void 0, void 0, function* () {
        let cards = new Array();
        let result = { msg: null, matched: false, match: {} };
        let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/;
        if (!spotifyArtistRegex.test(uri)) {
            throw 'Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported';
        }
        let artistId = uri.split(":")[2];
        let artistData = yield _spotify.getArtist(artistId);
        result.matched = true;
        result.match = {
            artistName: artistData.name,
            artistId: artistData.id,
            artistUri: artistData.uri
        };
        let card = new builder.HeroCard(session)
            .title(artistData.name)
            .images([builder.CardImage.create(session, artistData.images[0].url)]);
        cards.push(card);
        if (cards.length == 0)
            return result;
        let msg = new builder.Message(session);
        msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments(cards);
        result.msg = msg;
        return result;
    });
}
exports.getArtistByUri = getArtistByUri;
;
function getArtists(session, artists) {
    return __awaiter(this, void 0, void 0, function* () {
        let cards = new Array();
        let result = { msg: null, matched: false, match: { artistName: null, artistUid: null } };
        for (var i in artists) {
            let artist = artists[i];
            let artistData = yield _spotify.searchArtists(artist, 3);
            if (artistData.artists.items.length > 0) {
                //session.userData.faveArtist = artistData; // ??
                findMatch(artistData.artists.items, result);
                for (var j in artistData.artists.items) {
                    let item = artistData.artists.items[j];
                    // skip items with no images
                    if (item.images.length === 0)
                        continue;
                    let card = new builder.HeroCard(session)
                        .title(item.name)
                        .images([builder.CardImage.create(session, item.images[0].url)]);
                    if (!result.matched) {
                        card.text(`Do you like ${item.name}?`);
                        card.buttons([
                            builder.CardAction.imBack(session, `I like ${item.uri}`, `I like ${item.name}`)
                        ]);
                    }
                    cards.push(card);
                }
            }
        }
        if (cards.length == 0)
            return result;
        let msg = new builder.Message(session);
        msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments(cards);
        result.msg = msg;
        return result;
    });
}
exports.getArtists = getArtists;
;
function getRelatedArtists(session, artistId, limit = 1) {
    return __awaiter(this, void 0, void 0, function* () {
        let cards = new Array();
        let result = yield _spotify.getRelatedArtists(artistId);
        if (result.artists.length == 0)
            return null;
        // filter out artists without images and sort by popularity desc
        let artists = result.artists.filter(a => a.images.length > 0).sort((a, b) => {
            if (a.popularity > b.popularity)
                return -1;
            if (a.popularity < b.popularity)
                return 1;
            return 0;
        });
        for (var i = 0; i < limit; i++) {
            cards.push(new builder.HeroCard(session)
                .title(artists[i].name)
                .text(`Do you like ${artists[i].name}?`)
                .images([builder.CardImage.create(session, artists[i].images[0].url)])
                .buttons([
                builder.CardAction.imBack(session, `I like ${artists[i].uri}`, `I like ${artists[i].name}`)
            ]));
        }
        let msg = new builder.Message(session);
        msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments(cards);
        return msg;
    });
}
exports.getRelatedArtists = getRelatedArtists;
;
function getRecommendations(session, artistSeed) {
    return __awaiter(this, void 0, void 0, function* () {
        let cards = new Array();
        let recommendationData = yield _spotify.getRecommendation(artistSeed, 3);
        if (recommendationData.tracks.length > 0) {
            //let data = new Array(recommendationData.tracks);
            //let album = data.filter(item => item.
            recommendationData.tracks.forEach(album => {
                cards.push(new builder.HeroCard(session)
                    .title(album.name)
                    .text(`Do you like ${album.name} by ${album.artists[0].name}?`)
                    .images([builder.CardImage.create(session, album.album.images[0].url)])
                    .buttons([
                    builder.CardAction.imBack(session, `I like album ${album.name}`, `I like album ${album.name}`)
                ]));
            });
        }
        ;
        if (cards.length == 0)
            return;
        let msg = new builder.Message(session);
        msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments(cards);
        return msg;
    });
}
exports.getRecommendations = getRecommendations;
;
