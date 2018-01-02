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
        for (var i in artists) {
            let artist = artists[i];
            let artistData = yield _spotify.searchArtists(artist, 1);
            if (artistData.artists.items.length > 0) {
                let item = artistData.artists.items[0];
                cards.push(new builder.HeroCard(session)
                    .title(item.name)
                    .text(`Do you like ${item.name}?`)
                    .images([builder.CardImage.create(session, item.images[0].url)])
                    .buttons([
                    builder.CardAction.imBack(session, `I like ${item.name}`, `I like ${item.name}`)
                ]));
            }
        }
        if (cards.length == 0)
            return;
        let msg = new builder.Message(session);
        msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments(cards);
        return msg;
    });
}
exports.getArtists = getArtists;
;
