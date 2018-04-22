//import _table = require('./tablestorage');
import _canonical = require('./canonicalisation');
import _servicebus = require('./servicebus');

export async function userLikesArtist(user: string, artist: string) {
    if (!user) throw new Error('user cannot be null');
    
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
    }

    try{
        await _servicebus.sendMessage('graph', entityRelationship);
    } catch(e) {
        throw e;
    }
};
