//import _table = require('./tablestorage');
import _canonical = require('./canonicalisation');
import _servicebus = require('./servicebus');
import artist = require('../models/artist');
import _graph = require('./graphstorage');
import entityrelation = require('../models/entityrelation');
import * as _httpj from "./httpjson";

/**
 * Get all of the artists that a User likes
 * @param userId 
 */

export async function artistsUserLikes(userId) :Promise<artist.Artist[]> {
    let data = await _graph.vertexEdgeVertices(userId, 'likes', 'artist')
    return artist.mapGraphToArtists(data);
}

export async function userLikesArtist(userId: string, artist: artist.Artist) {
    if (!userId) throw new Error('user cannot be null');

    // graph artist Id
    let artistId = `${artist.name.toLowerCase()}:${_canonical.getArtistId(artist.name).Id}`;

   let erList:entityrelation.EntityRelationship[] = [];

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
    }

    erList.push(entityRelationship)

    try {
        let url = `${process.env.API_BACKEND}RelatedArtists_HttpStart`;
        await _httpj.post(url, JSON.stringify(erList), {"Content-Type": "application/json"});
    } catch (e) {
        throw e;
    }
};
