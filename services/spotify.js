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
const baseUrl = 'https://api.spotify.com/v1';
const tokenUrl = 'https://accounts.spotify.com/api/token';
let authToken = { token: null, expires: null };
function searchArtists(artist, limit) {
    return __awaiter(this, void 0, void 0, function* () {
        let token = yield getAuthToken();
        if (!limit || limit <= 0) {
            limit = 50;
        }
        // https://api.spotify.com/v1/search?q=radiohead&type=artist
        let response = yield _httpj.get(`${baseUrl}/search?q=${encodeURIComponent(artist)}&type=artist&limit=${limit}`, { 'Authorization': 'Bearer ' + token });
        return response;
    });
}
exports.searchArtists = searchArtists;
function getPlaylists(username, offset) {
    return __awaiter(this, void 0, void 0, function* () {
        let token = yield getAuthToken();
        let response = yield _httpj.get(baseUrl + '/users/' + encodeURIComponent(username.toLowerCase()) + '/playlists?limit=50&offset='
            + offset, { 'Authorization': 'Bearer ' + token });
        return response;
    });
}
exports.getPlaylists = getPlaylists;
;
function getRecommendation(artistSeed, limit) {
    return __awaiter(this, void 0, void 0, function* () {
        let token = yield getAuthToken();
        let response = yield _httpj.get(baseUrl + '/recommendations' + `?seed_artists=${artistSeed}` + `&limit=${limit}`, { 'Authorization': 'Bearer ' + token });
        return response;
    });
}
exports.getRecommendation = getRecommendation;
;
function getRelatedArtists(artistId) {
    return __awaiter(this, void 0, void 0, function* () {
        // GET https://api.spotify.com/v1/artists/{id}/related-artists
        let token = yield getAuthToken();
        let response = yield _httpj.get(`${baseUrl}/artists/${artistId}/related-artists`, { 'Authorization': 'Bearer ' + token });
        return response;
    });
}
exports.getRelatedArtists = getRelatedArtists;
;
function getAuthToken() {
    return __awaiter(this, void 0, void 0, function* () {
        let now = new Date();
        if (process.env.SpotifyApiClientId == undefined || process.env.SpotifyApiClientSecret == undefined) {
            throw new Error("spotify.js requires environment vars \"SpotifyApiClientId\" and \"SpotifyApiClientSecret\".");
        }
        if (authToken.expires == null || authToken.expires.getTime() < now.getTime()) {
            let authEncoded = new Buffer(process.env.SpotifyApiClientId + ':' + process.env.SpotifyApiClientSecret).toString('base64');
            // get a new token
            let response = yield _httpj.post(tokenUrl, "grant_type=client_credentials", { 'Authorization': 'Basic ' + authEncoded });
            console.log('got auth token');
            let expires = new Date(now.getTime() + response.expires_in * 1000);
            authToken = { token: response.access_token, expires: expires };
            return authToken.token;
        }
        return authToken.token;
    });
}
exports.getAuthToken = getAuthToken;
;
