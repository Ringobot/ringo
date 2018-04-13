import _spotify = require('./spotify');

export function searchArtists(artist:string, limit?:number) {
    return _spotify.searchArtists(artist, limit);
}

export function getRelatedArtists(artistId:string) {
    return _spotify.getRelatedArtists(artistId);
};

export function getArtist(artistId:string) {
    return _spotify.getArtist(artistId);
};

export function getArtistByUri(uri:string) {
    let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/

    if (!spotifyArtistRegex.test(uri)) {
        throw 'Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported';
    }

    let artistId = uri.split(":")[2]; 
    return _spotify.getArtist(artistId);
}
