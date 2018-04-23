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
    return __awaiter(this, void 0, void 0, function* () {
        let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/;
        if (!spotifyArtistRegex.test(uri)) {
            throw new Error('Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported');
        }
        let artistId = uri.split(":")[2];
        try {
            return yield artist.MapToArtist(_spotify.getArtist(artistId));
        }
        catch (e) {
            throw e;
        }
    });
}
exports.getArtistByUri = getArtistByUri;
