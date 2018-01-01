import _table = require('./tablestorage');

export async function userLikesArtist(user:string, artist:string) {
    let entity = { 
        PartitionKey: user.toLowerCase(), 
        RowKey: artist.toLowerCase(),
        User: user, 
        Artist: artist,
        WhenLiked: new Date()
    };

    await _table.insert('UserLikesArtist', entity, true);
};
