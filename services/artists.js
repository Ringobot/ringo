"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const _spotify = require("./spotify");
function searchArtists(artist, limit) {
    return _spotify.searchArtists(artist, limit);
}
exports.searchArtists = searchArtists;
function getRelatedArtists(artistId) {
    return _spotify.getRelatedArtists(artistId);
}
exports.getRelatedArtists = getRelatedArtists;
;
function getArtist(artistId) {
    return _spotify.getArtist(artistId);
}
exports.getArtist = getArtist;
;
function getArtistByUri(uri) {
    let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/;
    if (!spotifyArtistRegex.test(uri)) {
        throw 'Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported';
    }
    let artistId = uri.split(":")[2];
    return _spotify.getArtist(artistId);
}
exports.getArtistByUri = getArtistByUri;
