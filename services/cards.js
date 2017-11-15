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
function getArtists(session, artists, callback) {
    return __awaiter(this, void 0, void 0, function* () {
        var msg = new builder.Message(session);
        msg.attachmentLayout(builder.AttachmentLayout.carousel);
        msg.attachments([
            new builder.HeroCard(session)
                .title("Radiohead")
                .text("234 of my friends listen to Radiohead")
                .images([builder.CardImage.create(session, 'https://i.scdn.co/image/afcd616e1ef2d2786f47b3b4a8a6aeea24a72adc')])
                .buttons([
                builder.CardAction.messageBack(session, "This is \"Radiohead\"", "Yep"),
                builder.CardAction.messageBack(session, "This is not \"Radiohead\"", "Nope")
            ]),
            new builder.HeroCard(session)
                .title("Massive Attack")
                .text("2,011 of my friends listen to Massive Attack")
                .images([builder.CardImage.create(session, 'https://i.scdn.co/image/c8bbeedb05f38ae5cb982a7daf4bf7129cca892c')])
                .buttons([
                builder.CardAction.messageBack(session, "This is \"Massive Attack\"", "Yep"),
                builder.CardAction.messageBack(session, "This is not \"Massive Attack\"", "Nope")
            ])
        ]);
        callback(null, msg);
    });
}
exports.getArtists = getArtists;
;
