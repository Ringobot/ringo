import _table = require('./tablestorage');
import _graph = require('./graphstorage')
import _canonical = require('./canonicalisation');
import { createClient } from '../gremlin/index';

export async function userLikesArtist(user: string, artist: string) {
    if (!user) throw 'user cannot be null';
    
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

    var queryUserVertx = await _graph.getVertexByName(graphClient, userVertex.Name);
    console.log(queryUserVertx);
    if (queryUserVertx.length == 0){
        var userVertexResult = await _graph.addVertex(graphClient, userVertex);
    }
    else var userVertexResult = queryUserVertx;
    console.log(userVertexResult);
    
    var queryArtistVertx = await _graph.getVertexByName(graphClient, artistVertex.Name);
    console.log(queryArtistVertx);
    if (queryArtistVertx.length == 0){
        var artistVertexResult = await _graph.addVertex(graphClient, artistVertex);
    }
    else var artistVertexResult = queryArtistVertx;
    console.log(artistVertexResult);

        //ToDo make this accept vertex vs string
    var likesEdge = {
        FromVertex: userVertexResult[0].id,
        Relationship: 'likes',
        ToVertex: artistVertexResult[0].id
    }

    await _graph.addEdgeLike(graphClient, likesEdge);

};
