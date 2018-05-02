"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function mapToArtist(spotifyArtist) {
    if (!spotifyArtist)
        return null;
    if (!spotifyArtist.name && !spotifyArtist.id && !spotifyArtist.uri) {
        // it's not an artist
        throw new Error(`${spotifyArtist} is not a Spotify artist`);
    }
    return {
        name: spotifyArtist.name,
        spotify: {
            id: spotifyArtist.id,
            uri: spotifyArtist.uri
        },
        images: spotifyArtist.images
    };
}
exports.mapToArtist = mapToArtist;
function mapToArtists(spotifyData) {
    if (!spotifyData)
        return null;
    // spotify artist responses come in two shapes (!)
    let data = spotifyData.artists.items || spotifyData.artists;
    if (!data) {
        throw new Error(`${spotifyData} is not a valid Spotify artists response`);
    }
    return data.map(mapToArtist);
}
exports.mapToArtists = mapToArtists;
function mapGraphToArtists(data) {
    let artists = [];
    data.forEach(item => {
        artists.push({
            name: item.properties['name'][0].value,
            spotify: {
                id: item.properties['spotifyId'] && item.properties['spotifyId'][0].value,
                uri: item.properties['spotifyUri'] && item.properties['spotifyUri'][0].value
            },
            images: [{
                    height: undefined,
                    url: item.properties['imageUrl'] && item.properties['imageUrl'][0].value,
                    width: undefined
                }]
        });
    });
    return artists;
}
exports.mapGraphToArtists = mapGraphToArtists;
