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
const crypto = require("crypto");
const _table = require("./tablestorage");
//import restify = require('restify');
const errs = require("restify-errors");
const baseUrl = 'https://api.spotify.com/v1';
const tokenUrl = 'https://accounts.spotify.com/api/token';
const authUrl = 'https://accounts.spotify.com/authorize';
const scopes = 'user-modify-playback-state user-read-playback-state user-read-recently-played playlist-read-collaborative playlist-modify-public playlist-modify-private playlist-read-private user-follow-modify user-top-read';
let authToken = { token: null, expires: null };
function validateEnvVars() {
    if (process.env.SpotifyApiClientId == undefined || process.env.SpotifyApiClientSecret == undefined) {
        throw new Error("spotify.js requires environment vars \"SpotifyApiClientId\" and \"SpotifyApiClientSecret\".");
    }
    if (process.env.SpotifyAuthRedirectUri == undefined) {
        throw new Error("spotify.js requires environment var \"SpotifyAuthRedirectUri\".");
    }
}
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
function getArtist(artistId) {
    return __awaiter(this, void 0, void 0, function* () {
        // GET https://api.spotify.com/v1/artists/{id}
        let token = yield getAuthToken();
        let response = yield _httpj.get(`${baseUrl}/artists/${artistId}`, { 'Authorization': 'Bearer ' + token });
        return response;
    });
}
exports.getArtist = getArtist;
;
function getAuthToken() {
    return __awaiter(this, void 0, void 0, function* () {
        let now = new Date();
        validateEnvVars();
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
function partitionKey(userHash) {
    return userHash.substr(0, 5);
}
function authorize(req, res, next) {
    return __awaiter(this, void 0, void 0, function* () {
        validateEnvVars();
        // /authorize/spotify/60BC0AC2A44EA6146D876AEC3133D230B9A9E41BACC7EA0343B16FED4CB6BE54
        let reg = /[a-zA-Z0-9]{64}/;
        if (!reg.test(req.params.userHash))
            throw "Not a valid /authorize Path";
        let userAuth = {
            PartitionKey: partitionKey(req.params.userHash),
            RowKey: req.params.userHash,
            userHash: req.params.userHash
        };
        try {
            let result = yield _table.insert("UserSpotifyAuth", userAuth);
            // 60BC0AC2A44EA6146D876AEC3133D230B9A9E41BACC7EA0343B16FED4CB6BE54/f26d60305
            let state = `${req.params.userHash}/${result[".metadata"].etag}`;
            // https://accounts.spotify.com/authorize/?client_id=4b4a9fcb021a4d02a4acd1d8adba0bfe&response_type=code&redirect_uri=https%3A%2F%2Fexample.com%2Fcallback&scope=user-read-private%20user-read-email&state=34fFs29kd09
            let url = `${authUrl}/?client_id=${process.env.SpotifyApiClientId}&response_type=code&redirect_uri=${process.env.SpotifyAuthRedirectUri}&scope=${scopes}&state=${state}`;
            res.redirect(302, url, next);
            next();
        }
        catch (e) {
            console.error(e);
            next(new errs.InternalServerError());
        }
    });
}
exports.authorize = authorize;
function sha1(input) {
    let sha = crypto.createHash('sha1');
    sha.update(input);
    return sha.digest('hex');
}
function sha256(input) {
    let sha = crypto.createHash('sha256');
    sha.update(input);
    return sha.digest('hex');
}
function randomString(length = 40) {
    // https://stackoverflow.com/a/14869745/610731
    return crypto.randomBytes(length / 2).toString('hex');
}
function userAuth(userId) {
    return __awaiter(this, void 0, void 0, function* () {
        let userHash = sha256(userId);
        let partition = userHash.substr(0, 5);
        let result = {
            authorised: false,
            userHash: userHash,
            auth: null
        };
        let query = _table.createQuery()
            .where('PartitionKey eq ?', partition)
            .where('RowId eq ?', userHash);
        let row = yield _table.get("UserSpotifyAuth", query);
        if (row) {
            result.authorised = true;
            result.auth = row;
        }
        return result;
    });
}
exports.userAuth = userAuth;
function getUserAuthToken(userAuth) {
    return __awaiter(this, void 0, void 0, function* () {
        let now = new Date();
        if (process.env.SpotifyApiClientId == undefined || process.env.SpotifyApiClientSecret == undefined) {
            throw new Error("spotify.js requires environment vars \"SpotifyApiClientId\" and \"SpotifyApiClientSecret\".");
        }
        if (process.env.SpotifyAuthRedirectUri == undefined) {
            throw new Error("spotify.js requires environment var \"SpotifyAuthRedirectUri\".");
        }
        if (userAuth.auth.expires == null || userAuth.auth.expires.getTime() < now.getTime()) {
            let authEncoded = new Buffer(process.env.SpotifyApiClientId + ':' + process.env.SpotifyApiClientSecret).toString('base64');
            // get a new token
            let response = yield _httpj.post(tokenUrl, `grant_type=authorization_code;code=${userAuth.auth.authCode};redirect_uri=${process.env.SpotifyAuthRedirectUri}`, { 'Authorization': 'Basic ' + authEncoded });
            console.log('got auth token');
            let expires = new Date(now.getTime() + response.expires_in * 1000);
            authToken = { token: response.access_token, expires: expires };
            return authToken.token;
        }
        return userAuth.auth.token;
    });
}
exports.getUserAuthToken = getUserAuthToken;
;
function playArtist(userAuth, spotifyUri) {
    return __awaiter(this, void 0, void 0, function* () {
        // PUT https://api.spotify.com/v1/me/player/play
        let token = yield getUserAuthToken(userAuth);
        /*
        let response = await _httpj.put(`${baseUrl}/me/player/play`,
            { 'Authorization': 'Bearer ' + token });
    
        return response;
        */
    });
}
exports.playArtist = playArtist;
