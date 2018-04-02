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
function getArtists(session, artists) {
    return __awaiter(this, void 0, void 0, function* () {
        let cards = new Array();
        let result = { msg: null, oneResult: false };
        for (var i in artists) {
            let artist = artists[i];
            let artistData = yield _spotify.searchArtists(artist, 3);
            if (artistData.artists.items.length > 0) {
                session.userData.faveArtist = artistData; // ??
                result.oneResult = artistData.artists.items.filter(i => i.images.length > 0).length === 1;
                for (var j in artistData.artists.items) {
                    let item = artistData.artists.items[j];
                    // skip items with no images
                    if (item.images.length === 0)
                        continue;
                    let card = new builder.HeroCard(session)
                        .title(item.name)
                        .images([builder.CardImage.create(session, item.images[0].url)]);
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
        if (cards.length == 0)
            return;
        let msg = new builder.Message(session);
        msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments(cards);
        result.msg = msg;
        return result;
    });
}
exports.getArtists = getArtists;
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
