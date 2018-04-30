//import _table = require('./tablestorage');
import _canonical = require('./canonicalisation');
import _servicebus = require('./servicebus');
import artist = require('../models/artist');

export async function userLikesArtist(userId: string, artist: artist.Artist) {
    if (!userId) throw new Error('user cannot be null');

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
    }

    try {
        await _servicebus.sendMessage('graph', entityRelationship);
    } catch (e) {
        throw e;
    }
};
