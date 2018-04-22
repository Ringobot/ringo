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
const _httpj = require("./httpjson");
const _auth = require("./spotifyAuth");
const baseUrl = 'https://api.spotify.com/v1';
function searchArtists(artist, limit) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            let token = yield _auth.getClientAuthToken();
            if (!limit || limit <= 0) {
                limit = 50;
            }
            // https://api.spotify.com/v1/search?q=radiohead&type=artist
            let response = yield _httpj.get(`${baseUrl}/search?q=${encodeURIComponent(artist)}&type=artist&limit=${limit}`, { 'Authorization': 'Bearer ' + token });
            return response;
        }
        catch (e) {
            throw e;
        }
    });
}
exports.searchArtists = searchArtists;
function getPlaylists(username, offset) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            let token = yield _auth.getClientAuthToken();
            let response = yield _httpj.get(baseUrl + '/users/' + encodeURIComponent(username.toLowerCase()) + '/playlists?limit=50&offset='
                + offset, { 'Authorization': 'Bearer ' + token });
            return response;
        }
        catch (e) {
            throw e;
        }
    });
}
exports.getPlaylists = getPlaylists;
function getRecommendation(artistSeed, limit) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            let token = yield _auth.getClientAuthToken();
            let response = yield _httpj.get(baseUrl + '/recommendations' + `?seed_artists=${artistSeed}` + `&limit=${limit}`, { 'Authorization': 'Bearer ' + token });
            return response;
        }
        catch (e) {
            throw e;
        }
    });
}
exports.getRecommendation = getRecommendation;
function getRelatedArtists(artistId) {
    return __awaiter(this, void 0, void 0, function* () {
        // GET https://api.spotify.com/v1/artists/{id}/related-artists
        try {
            let token = yield _auth.getClientAuthToken();
            let response = yield _httpj.get(`${baseUrl}/artists/${artistId}/related-artists`, { 'Authorization': 'Bearer ' + token });
            return response;
        }
        catch (e) {
            throw e;
        }
    });
}
exports.getRelatedArtists = getRelatedArtists;
;
function getArtist(artistId) {
    return __awaiter(this, void 0, void 0, function* () {
        // GET https://api.spotify.com/v1/artists/{id}
        try {
            let token = yield _auth.getClientAuthToken();
            let response = yield _httpj.get(`${baseUrl}/artists/${artistId}`, { 'Authorization': 'Bearer ' + token });
            return response;
        }
        catch (e) {
            throw e;
        }
    });
}
exports.getArtist = getArtist;
;
function playArtist(userHash, spotifyUri) {
    return __awaiter(this, void 0, void 0, function* () {
        // PUT https://api.spotify.com/v1/me/player/play
        try {
            // TODO: -> getUserAuthHeader
            let token = yield _auth.getUserAuthToken(userHash);
            let json = spotifyUri ? `{\r\n  \"context_uri\": \"${spotifyUri}\"\r\n}` : null;
            //let data = { context_uri: spotifyUri };
            console.log(json);
            yield _httpj.put(`${baseUrl}/me/player/play`, json, { 'Authorization': 'Bearer ' + token });
        }
        catch (e) {
            throw e;
        }
    });
}
exports.playArtist = playArtist;
