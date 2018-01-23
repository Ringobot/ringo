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
const _table = require("./tablestorage");
const _graph = require("./graphstorage");
const _canonical = require("./canonicalisation");
function userLikesArtist(user, artist) {
    return __awaiter(this, void 0, void 0, function* () {
        let entity = {
            PartitionKey: user.toLowerCase(),
            RowKey: _canonical.getArtistId(artist).Id,
            User: user,
            Artist: artist,
            WhenLiked: new Date()
        };
        yield _table.insert('UserLikesArtist', entity, true);
        var graphClient = _graph.createClient();
        var userVertex = {
            Name: user,
            Properties: []
        };
        var artistVertex = {
            Name: artist,
            Properties: []
        };
        //ToDo make this accept vertex vs string
        var likesEdge = {
            FromVertex: userVertex.Name,
            Relationship: 'likes',
            ToVertex: artistVertex.Name
        };
        yield _graph.addVertex(graphClient, userVertex);
        yield _graph.addVertex(graphClient, artistVertex);
        yield _graph.addEdgeLike(graphClient, likesEdge);
    });
}
exports.userLikesArtist = userLikesArtist;
;
