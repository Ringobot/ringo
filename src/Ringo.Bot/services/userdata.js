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
//import _table = require('./tablestorage');
const _canonical = require("./canonicalisation");
const _servicebus = require("./servicebus");
function userLikesArtist(user, artist) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!user)
            throw new Error('user cannot be null');
        let userId = user;
        let artistId = `${artist.toLowerCase()}:${_canonical.getArtistId(artist).Id}`;
        let entity = {
            PartitionKey: userId,
            RowKey: artistId,
            User: user,
            Artist: artist,
            WhenLiked: new Date()
        };
        //await _table.insert('UserLikesArtist', entity, true);
        var entityRelationship = {
            FromVertex: {
                Id: userId,
                Name: user,
                Properties: {
                    type: "user"
                }
            },
            ToVertex: {
                Id: artistId,
                Name: artist,
                Properties: {
                    type: "artist"
                }
            },
            Relationship: "likes",
            RelationshipDate: new Date()
        };
        try {
            yield _servicebus.sendMessage('graph', entityRelationship);
        }
        catch (e) {
            throw e;
        }
    });
}
exports.userLikesArtist = userLikesArtist;
;
