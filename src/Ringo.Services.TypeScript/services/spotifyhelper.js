"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function validateEnvVars() {
    if (process.env.SpotifyApiClientId == undefined || process.env.SpotifyApiClientSecret == undefined) {
        throw new Error("spotify.js requires environment vars \"SpotifyApiClientId\" and \"SpotifyApiClientSecret\".");
    }
    if (process.env.SpotifyAuthRedirectUri == undefined) {
        throw new Error("spotify.js requires environment var \"SpotifyAuthRedirectUri\".");
    }
}
exports.validateEnvVars = validateEnvVars;
