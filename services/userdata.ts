import _table = require('./tablestorage');
import _graph = require('./graphstorage')
import _canonical = require('./canonicalisation');
import { createClient } from '../gremlin/index';

export async function userLikesArtist(user: string, artist: string) {
    let entity = {
        PartitionKey: user.toLowerCase(),
        RowKey: _canonical.getArtistId(artist).Id,
        User: user,
        Artist: artist,
        WhenLiked: new Date()
    };

    await _table.insert('UserLikesArtist', entity, true);

    var graphClient = _graph.createClient();

    var userVertex = {
        Name: user,
        Properties: []
    }

    var artistVertex = {
        Name: artist,
        Properties: []
    }

    //ToDo make this accept vertex vs string
    var likesEdge = {
        FromVertex: userVertex.Name,
        Relationship: 'likes',
        ToVertex: artistVertex.Name
    }

    await _graph.addVertex(graphClient, userVertex)
    await _graph.addVertex(graphClient, artistVertex)
    await _graph.addEdgeLike(graphClient, likesEdge)

};
