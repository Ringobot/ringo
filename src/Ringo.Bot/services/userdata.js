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
        if (!user)
            throw 'user cannot be null';
        let userId = user;
        let artistId = `${artist.toLowerCase()}:${_canonical.getArtistId(artist).Id}`;
        let entity = {
            PartitionKey: userId,
            RowKey: artistId,
            User: user,
            Artist: artist,
            WhenLiked: new Date()
        };
        yield _table.insert('UserLikesArtist', entity, true);
        var graphClient = _graph.createClient();
        var userVertex = {
            Id: userId,
            Name: user,
            Properties: []
        };
        var artistVertex = {
            Id: artistId,
            Name: artist,
            Properties: []
        };
        // TODO: Get vertex by Id?
        var queryUserVertx = yield _graph.getVertexById(graphClient, userVertex.Id);
        console.log(queryUserVertx);
        if (queryUserVertx.length == 0) {
            var userVertexResult = yield _graph.addVertex(graphClient, userVertex);
        }
        else
            var userVertexResult = queryUserVertx;
        console.log(userVertexResult);
        var queryArtistVertx = yield _graph.getVertexById(graphClient, artistVertex.Id);
        console.log(queryArtistVertx);
        if (queryArtistVertx.length == 0) {
            var artistVertexResult = yield _graph.addVertex(graphClient, artistVertex);
        }
        else
            var artistVertexResult = queryArtistVertx;
        console.log(artistVertexResult);
        //ToDo make this accept vertex vs string
        var likesEdge = {
            FromVertex: userVertexResult[0].id,
            Relationship: 'likes',
            ToVertex: artistVertexResult[0].id
        };
        yield _graph.addEdgeLike(graphClient, likesEdge);
    });
}
exports.userLikesArtist = userLikesArtist;
;
