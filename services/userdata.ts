import _table = require('./tablestorage');
import _canonical = require('./canonicalisation');

export async function userLikesArtist(user: string, artist: string) {
    let entity = {
        PartitionKey: user.toLowerCase(),
        RowKey: _canonical.getArtistId(artist).Id,
        User: user,
        Artist: artist,
        WhenLiked: new Date()
    };

    await _table.insert('UserLikesArtist', entity, true);
};
