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
const UserAuthTable = "UserSpotifyAuth";
const baseUrl = 'https://api.spotify.com/v1';
const tokenUrl = 'https://accounts.spotify.com/api/token';
const authUrl = 'https://accounts.spotify.com/authorize';
const scopes = ascii('user-modify-playback-state user-read-playback-state user-read-recently-played playlist-read-collaborative playlist-modify-public playlist-modify-private playlist-read-private user-follow-modify user-top-read');
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
let rowQuery = (userHash) => _table.createQuery().where('PartitionKey eq ? and RowKey eq ?', partitionKey(userHash), userHash);
function authorizeCallback(req, res, next) {
    return __awaiter(this, void 0, void 0, function* () {
        // https://ringobot.azurewebsites.net/authorize/spotify?code=AQAYtuv4d6NsFQYBbYv-gmzI0K1_LDUjpBNe59yC1pID0Yl6LTLtcJ3kPtOu0jkRH4TxDCEXAAWbeoQ72DAmzug5LFnKyoP-cOT7NvzC4IMqlavrzgonrjSL_-B1uIA3uo8Lzgds1TWRaqPf304axiUc0ivvxWjQSjlRkj2rcHe2inCcoalRQEvAa4ZMvkVoZ7KFJcXERlGZkS17LkRIJnhuthVK55cfWGkTDgHWDWlamfz4Lb3uvQHElk-OVP6a1YTOn2IfxUGgFtAx7CC0Vqw5fS37L7ONdMmcTqDENpQ1wCGaRfHJ2b6t7JZx88DbrRiaH8KjYFShw4f2wDOI9wyEusOPhjnngCUGZu17kzcIeZcJFZRCO5hiSGcMTxb020m_mt7qK0PFJUOjHDT_gsHGsRz4dqwFjnOz12ThehFE8dvU7J9X9JOv4QeJ8Keg8kogz7keM76z1E8xi3svAUPd06m9-nsSCvZfNNS4G2QVGcOIFTsG3KyiIZkPfclNej7r&state=60BC0AC2A44EA6146D876AEC3133D230B9A9E41BACC7EA0343B16FED4CB6BE55%3A7dff3dc47b10121834e6715ae4deda2545cc8c59
        let state = req.query.state;
        if (!state || state.length === 0) {
            console.error("Expecting state in querystring");
            next(new errs.BadRequestError());
            return;
        }
        // split into userHash and state
        let parts = state.split(":");
        if (parts.length != 2) {
            console.error("Invalid state format");
            next(new errs.BadRequestError());
            return;
        }
        // get the auth record by userHash
        try {
            let entities = yield _table.get(UserAuthTable, rowQuery(parts[0]));
            if (entities.length !== 1) {
                // no entity found
                console.error("No matching row for " + parts[0]);
                next(new errs.BadRequestError());
                return;
            }
            let entity = entities[0];
            if (parts[1] !== entity.state) {
                // states don't match
                console.error("states don't match");
                next(new errs.BadRequestError());
                return;
            }
            let error = req.query.error;
            if (error) {
                // auth request was rejected or failed.
                console.error(`Authorization failed. Spotify error = ${error}`);
                // record error
                entity.error = `${new Date()}: Authorization failed. Spotify error = ${error}`;
                yield _table.update(UserAuthTable, entity);
                next(new errs.UnauthorizedError(`Authorization failed: ${error}`));
                return;
            }
            let code = req.query.code;
            if (!code || code.length == 0) {
                // no code in query
                console.error("Expecting code in querystring");
                next(new errs.BadRequestError());
                return;
            }
            // update the userAuth record just in case we need to retry
            entity.code = code;
            yield _table.update(UserAuthTable, entity);
            // POST the code to get the tokens
            let result = yield getUserAuthTokens(entity.code);
            // Save the tokens in the auth table, delete the state and code
            entity.accessToken = result.accessToken;
            entity.authUrl = result.authUrl;
            entity.tokenType = result.tokenType;
            entity.scope = result.scope;
            entity.expiry = result.expires;
            entity.refreshToken = result.refreshToken;
            entity.state = null;
            entity.code = null;
            yield _table.update(UserAuthTable, entity);
            // TODO: let the user know???
            res.send(200, "Spotify Authorised Ok");
            return next();
        }
        catch (e) {
            console.error(e);
            next(new errs.InternalServerError());
        }
    });
}
exports.authorizeCallback = authorizeCallback;
function getUserAuthTokens(code) {
    return __awaiter(this, void 0, void 0, function* () {
        validateEnvVars();
        let response = yield _httpj.post(tokenUrl, `grant_type=authorization_code&code=${code}&redirect_uri=${process.env.SpotifyAuthRedirectUri}`, addAuthHeader());
        console.log('got auth token');
        let authTokens = {
            authUrl: tokenUrl,
            accessToken: response.access_token,
            tokenType: response.token_type,
            scope: response.scope,
            expiresIn: response.expires_in,
            expires: expires(response.expires_in),
            refreshToken: response.refresh_token
        };
        return authTokens;
    });
}
exports.getUserAuthTokens = getUserAuthTokens;
function getUserAuthToken(userAuth) {
    return __awaiter(this, void 0, void 0, function* () {
        let now = new Date();
        validateEnvVars();
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
function authorize(req, res, next) {
    return __awaiter(this, void 0, void 0, function* () {
        validateEnvVars();
        // https://ringobot.azurewebsites.net/authorize/spotify
        // /authorize/spotify/60BC0AC2A44EA6146D876AEC3133D230B9A9E41BACC7EA0343B16FED4CB6BE54
        let reg = /[a-zA-Z0-9]{64}/;
        if (!reg.test(req.params.userHash)) {
            next(new errs.BadRequestError());
            return;
        }
        let userAuth = {
            PartitionKey: partitionKey(req.params.userHash),
            RowKey: req.params.userHash,
            state: randomString(),
            userHash: req.params.userHash
        };
        try {
            let result = yield _table.insert(UserAuthTable, userAuth, true);
            // 60BC0AC2A44EA6146D876AEC3133D230B9A9E41BACC7EA0343B16FED4CB6BE54/f26d60305
            let state = `${req.params.userHash}:${userAuth.state}`;
            // https://accounts.spotify.com/authorize/?client_id=4b4a9fcb021a4d02a4acd1d8adba0bfe&response_type=code&redirect_uri=https%3A%2F%2Fexample.com%2Fcallback&scope=user-read-private%20user-read-email&state=34fFs29kd09
            let url = `${authUrl}/?client_id=${process.env.SpotifyApiClientId}&response_type=code&redirect_uri=${process.env.SpotifyAuthRedirectUri}&scope=${scopes}&state=${state}`;
            res.redirect(302, url, next);
        }
        catch (e) {
            console.error(e);
            next(new errs.InternalServerError());
        }
    });
}
exports.authorize = authorize;
function expires(expiresIn, now = new Date()) {
    return new Date(now.getTime() + expiresIn * 1000);
}
function addAuthHeader(headers = {}) {
    let authEncoded = Buffer.from(process.env.SpotifyApiClientId + ':' + process.env.SpotifyApiClientSecret).toString('base64');
    let result = new Object(headers);
    result['Authorization'] = 'Basic ' + authEncoded;
    return result;
}
function ascii(input) {
    return Buffer.from(input).toString("ascii");
}
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
function base64(input) {
    return Buffer.from(input).toString('base64');
}
function unBase64(input) {
    return Buffer.from(input, 'base64').toString('ascii');
}
/**
 * Get's an authorisation record for a User ID
 * @param userId
 */
function getUserAuthorization(userId) {
    return __awaiter(this, void 0, void 0, function* () {
        // hash the user ID
        let userHash = sha256(userId);
        let result = {
            authorised: false,
            userHash: userHash,
            auth: null
        };
        let entities = yield _table.get(UserAuthTable, rowQuery(userHash));
        // user is authorised if there is an access token
        result.authorised = (entities.length === 1 && entities[0].accessToken != null);
        result.auth = entities[0];
        return result;
    });
}
exports.getUserAuthorization = getUserAuthorization;
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
