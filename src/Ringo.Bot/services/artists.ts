import _spotify = require('./spotify');
import artist = require('../models/artist');

export function searchArtists(artist:string, limit?:number) {
    return _spotify.searchArtists(artist, limit);
}

export function getRelatedArtists(artistId:string) {
    return _spotify.getRelatedArtists(artistId);
};

export function getArtist(artistId:string) {
    return _spotify.getArtist(artistId);
};

export async function getArtistByUri(uri:string) : Promise<artist.Artist> {
    let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/

    if (!spotifyArtistRegex.test(uri)) {
        throw new Error('Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported');
    }

    let artistId = uri.split(":")[2];

    try {
        return await artist.MapToArtist(_spotify.getArtist(artistId));
    } catch (e) {
        throw e;
    }
}
