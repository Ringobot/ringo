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
function userLikesArtist(userId, artist) {
    return __awaiter(this, void 0, void 0, function* () {
        if (!userId)
            throw new Error('user cannot be null');
        // graph artist Id
        let artistId = `${artist.name.toLowerCase()}:${_canonical.getArtistId(artist.name).Id}`;
        /*
        let entity = {
            PartitionKey: userId,
            RowKey: artistId,
            User: userId,
            Artist: artist,
            WhenLiked: new Date()
        };
    
        //await _table.insert('UserLikesArtist', entity, true);
        */
        var entityRelationship = {
            FromVertex: {
                Id: userId,
                Name: userId,
                Properties: {
                    type: "user"
                }
            },
            ToVertex: {
                Id: artistId,
                Name: artist.name,
                Properties: {
                    type: "artist",
                    spotifyId: artist.spotify.id,
                    spotifyUri: artist.spotify.uri,
                    imageUrl: artist.images[0].url
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
