"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const _spotify = require("./spotify");
const artist = require("../models/artist");
function searchArtists(artistName, limit) {
    return __awaiter(this, void 0, void 0, function* () {
        return artist.MapToArtists(yield _spotify.searchArtists(artistName, limit));
    });
}
exports.searchArtists = searchArtists;
function getRelatedArtists(artistId) {
    return __awaiter(this, void 0, void 0, function* () {
        return artist.MapToArtists(yield _spotify.getRelatedArtists(artistId));
    });
}
exports.getRelatedArtists = getRelatedArtists;
;
function getArtist(artistId) {
    return _spotify.getArtist(artistId);
}
exports.getArtist = getArtist;
;
function getArtistByUri(uri) {
    return __awaiter(this, void 0, void 0, function* () {
        let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/;
        if (!spotifyArtistRegex.test(uri)) {
            throw new Error('Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported');
        }
        let artistId = uri.split(":")[2];
        try {
            return yield artist.MapToArtist(yield _spotify.getArtist(artistId));
        }
        catch (e) {
            throw e;
        }
    });
}
exports.getArtistByUri = getArtistByUri;
/**
 * Finds true and the artist if only one artist (with an image) is found in the array
 * @param artists an array of Artists (returned by a search API)
 */
function findMatch(artists) {
    let artistsWithImages = artists.filter(i => i.images.length > 0);
    return artistsWithImages.length === 1 ? [true, artistsWithImages[0]] : [false, null];
}
exports.findMatch = findMatch;
