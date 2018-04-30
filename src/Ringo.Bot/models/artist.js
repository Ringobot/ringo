"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function MapToArtist(spotifyArtist) {
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
exports.MapToArtist = MapToArtist;
function MapToArtists(spotifyData) {
    if (!spotifyData)
        return null;
    // spotify artist responses come in two shapes (!)
    let data = spotifyData.artists.items || spotifyData.artists;
    if (!data) {
        throw new Error(`${spotifyData} is not a valid Spotify artists response`);
    }
    return data.map(MapToArtist);
}
exports.MapToArtists = MapToArtists;
