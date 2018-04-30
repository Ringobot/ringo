import * as _httpj from "./httpjson";
import _table = require('./tablestorage');
import errs = require('restify-errors');
import _helper = require('./spotifyHelper');
import _auth = require('./spotifyauth');
import _crypto = require('../helpers/crypto')

const baseUrl = 'https://api.spotify.com/v1';

export async function searchArtists(artist: string, limit?: number) {
    try {
        let token = await _auth.getClientAuthToken();

        if (!limit || limit <= 0) {
            limit = 50;
        }

        // https://api.spotify.com/v1/search?q=radiohead&type=artist

        let response = await _httpj.get(`${baseUrl}/search?q=${encodeURIComponent(artist)}&type=artist&limit=${limit}`,
            { 'Authorization': 'Bearer ' + token });
        return response;
    } catch (e) {
        throw e;
    }

}

export async function getPlaylists(username: string, offset: number) {
    try {
        let token = await _auth.getClientAuthToken();

        let response = await _httpj.get(baseUrl + '/users/' + encodeURIComponent(username.toLowerCase()) + '/playlists?limit=50&offset='
            + offset, { 'Authorization': 'Bearer ' + token });

        return response;
    } catch (e) {
        throw e;
    }

}

export async function getRecommendation(artistSeed: string, limit?: number) {
    try {
        let token = await _auth.getClientAuthToken();

        let response = await _httpj.get(baseUrl + '/recommendations' + `?seed_artists=${artistSeed}` + `&limit=${limit}`,
            { 'Authorization': 'Bearer ' + token });

        return response;
    } catch (e) {
        throw e;
    }

}

export async function getRelatedArtists(artistId: string) {
    // GET https://api.spotify.com/v1/artists/{id}/related-artists

    try {
        let token = await _auth.getClientAuthToken();

        let response = await _httpj.get(`${baseUrl}/artists/${artistId}/related-artists`,
            { 'Authorization': 'Bearer ' + token });

        return response;
    } catch (e) {
        throw e;
    }
};

export async function getArtist(artistId: string) {
    // GET https://api.spotify.com/v1/artists/{id}

    try {
        let token = await _auth.getClientAuthToken();

        let response = await _httpj.get(`${baseUrl}/artists/${artistId}`,
            { 'Authorization': 'Bearer ' + token });

        return response;
    } catch (e) {
        throw e;
    }
};


export async function playArtist(userHash: string, spotifyUri: string) {
    // PUT https://api.spotify.com/v1/me/player/play

    try {
        // TODO: -> getUserAuthHeader
        let token = await _auth.getUserAuthToken(userHash);
        let json = spotifyUri ? `{\r\n  \"context_uri\": \"${spotifyUri}\"\r\n}` : null;

        await _httpj.put(`${baseUrl}/me/player/play`,
            json, { 'Authorization': 'Bearer ' + token });

    } catch (e) {
        throw e;
    }
}