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
const errs = require("restify-errors");
const _helper = require("./spotifyhelper");
const _crypto = require("../helpers/crypto");
const _data = require("../data/spotifyUserAuthData");
const tokenUrl = 'https://accounts.spotify.com/api/token';
const authUrl = 'https://accounts.spotify.com/authorize';
const scopes = _crypto.ascii('user-modify-playback-state user-read-playback-state user-read-recently-played playlist-read-collaborative playlist-modify-public playlist-modify-private playlist-read-private user-follow-modify user-top-read');
let _clientAuthToken = { token: null, expires: null };
_helper.validateEnvVars();
function getClientAuthToken() {
    return __awaiter(this, void 0, void 0, function* () {
        let now = new Date();
        if (_clientAuthToken.expires == null || _clientAuthToken.expires.getTime() < now.getTime()) {
            // get a new token
            let response = yield _httpj.post(tokenUrl, "grant_type=client_credentials", addClientAuthHeader());
            console.log('got auth token');
            let expires = new Date(now.getTime() + response.expires_in * 1000);
            _clientAuthToken = { token: response.access_token, expires: expires };
            return _clientAuthToken.token;
        }
        return _clientAuthToken.token;
    });
}
exports.getClientAuthToken = getClientAuthToken;
;
function authorizeCallback(req, res, next) {
    return __awaiter(this, void 0, void 0, function* () {
        // https://ringobot.azurewebsites.net/authorize/spotify?code=AQAYtuv4d6NsFQYBbYv-gmzI0K1_LDUjpBNe59yC1pID0Yl6LTLtcJ3kPtOu0jkRH4TxDCEXAAWbeoQ72DAmzug5LFnKyoP-cOT7NvzC4IMqlavrzgonrjSL_-B1uIA3uo8Lzgds1TWRaqPf304axiUc0ivvxWjQSjlRkj2rcHe2inCcoalRQEvAa4ZMvkVoZ7KFJcXERlGZkS17LkRIJnhuthVK55cfWGkTDgHWDWlamfz4Lb3uvQHElk-OVP6a1YTOn2IfxUGgFtAx7CC0Vqw5fS37L7ONdMmcTqDENpQ1wCGaRfHJ2b6t7JZx88DbrRiaH8KjYFShw4f2wDOI9wyEusOPhjnngCUGZu17kzcIeZcJFZRCO5hiSGcMTxb020m_mt7qK0PFJUOjHDT_gsHGsRz4dqwFjnOz12ThehFE8dvU7J9X9JOv4QeJ8Keg8kogz7keM76z1E8xi3svAUPd06m9-nsSCvZfNNS4G2QVGcOIFTsG3KyiIZkPfclNej7r&state=60BC0AC2A44EA6146D876AEC3133D230B9A9E41BACC7EA0343B16FED4CB6BE55%3A7dff3dc47b10121834e6715ae4deda2545cc8c59
        let stateParam = req.query.state;
        if (!stateParam || stateParam.length === 0) {
            console.error("Expecting state in querystring");
            next(new errs.BadRequestError());
            return;
        }
        // split into userHash and state
        let parts = stateParam.split(":");
        if (parts.length != 2) {
            console.error("Invalid state format");
            next(new errs.BadRequestError());
            return;
        }
        let userHash = parts[0];
        let state = parts[1];
        // get the auth record by userHash
        try {
            let entity = yield _data.get(userHash);
            if (!entity) {
                // no entity found
                console.error("No matching row for " + userHash);
                next(new errs.BadRequestError());
                return;
            }
            if (state !== entity.state) {
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
                yield _data.get(userHash);
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
            yield _data.update(entity);
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
            yield _data.update(entity);
            // TODO: let the user know???
            res.send(200, "Spotify Authorised Ok. Now you can ask Ringo to Play music 👍");
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
        let response = yield _httpj.post(tokenUrl, `grant_type=authorization_code&code=${code}&redirect_uri=${process.env.SpotifyAuthRedirectUri}`, addClientAuthHeader());
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
function getUserAuthToken(userHash) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            // get userAuth record
            let userAuth = yield _data.get(userHash);
            if (!userAuth)
                throw new Error('Not Authorised'); // warning: catching on this exact message in app.js
            if (userAuth.expiry == null || userAuth.expiry.getTime() < new Date().getTime()) {
                // if expired, refresh the token
                let now = new Date();
                let response = yield _httpj.post(tokenUrl, `grant_type=refresh_token&refresh_token=${userAuth.refreshToken}&redirect_uri=${process.env.SpotifyAuthRedirectUri}`, addClientAuthHeader());
                console.log('refreshed token');
                let expires = new Date(now.getTime() + response.expires_in * 1000);
                userAuth.accessToken = response.access_token;
                userAuth.expiry = expires;
                // update the userAuth record
                yield _data.update(userAuth);
            }
            return userAuth.accessToken;
        }
        catch (e) {
            throw e;
        }
    });
}
exports.getUserAuthToken = getUserAuthToken;
;
function authorize(req, res, next) {
    return __awaiter(this, void 0, void 0, function* () {
        // https://ringobot.azurewebsites.net/authorize/spotify
        // /authorize/spotify/60BC0AC2A44EA6146D876AEC3133D230B9A9E41BACC7EA0343B16FED4CB6BE54
        let reg = /[a-zA-Z0-9]{64}/;
        if (!reg.test(req.params.userHash)) {
            next(new errs.BadRequestError());
            return;
        }
        let userAuth = _data.createUserAuthEntity(req.params.userHash, _crypto.randomString());
        try {
            let result = yield _data.insertOrReplace(userAuth);
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
function addClientAuthHeader(headers = {}) {
    let authEncoded = Buffer.from(process.env.SpotifyApiClientId + ':' + process.env.SpotifyApiClientSecret).toString('base64');
    let result = new Object(headers);
    result['Authorization'] = 'Basic ' + authEncoded;
    return result;
}
