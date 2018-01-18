import * as _httpj from "./httpjson";

const baseUrl = 'https://api.spotify.com/v1';
const tokenUrl = 'https://accounts.spotify.com/api/token';
let authToken = { token: null, expires: null };

export async function searchArtists(artist:string, limit?:number) {
    let token = await getAuthToken();
    
    if (!limit || limit <= 0){
        limit = 50;
    }

    // https://api.spotify.com/v1/search?q=radiohead&type=artist

    let response = await _httpj.get(`${baseUrl}/search?q=${encodeURIComponent(artist)}&type=artist&limit=${limit}`, 
                            { 'Authorization': 'Bearer ' + token });
    return response;
}

export async function getPlaylists(username: string, offset: number) {
    let token = await getAuthToken();

    let response = await _httpj.get(baseUrl + '/users/' + encodeURIComponent(username.toLowerCase()) + '/playlists?limit=50&offset='
        + offset, { 'Authorization': 'Bearer ' + token });

    return response;
};

export async function getRecommendation(artistSeed:string, limit?:number) {
    let token = await getAuthToken();

    let response = await _httpj.get(baseUrl + '/recommendations' + `?seed_artists=${artistSeed}` + `&limit=${limit}`, 
    { 'Authorization': 'Bearer ' + token });

    return response;
};

export async function getAuthToken() {
    let now = new Date();

    if (process.env.SpotifyApiClientId == undefined || process.env.SpotifyApiClientSecret == undefined) {
        throw new Error("spotify.js requires environment vars \"SpotifyApiClientId\" and \"SpotifyApiClientSecret\".");
    }

    if (authToken.expires == null || authToken.expires.getTime() < now.getTime()) {
        let authEncoded = new Buffer(process.env.SpotifyApiClientId + ':' + process.env.SpotifyApiClientSecret).toString('base64');

        // get a new token
        let response = await _httpj.post(tokenUrl, "grant_type=client_credentials",
            { 'Authorization': 'Basic ' + authEncoded });

        console.log('got auth token');
        let expires = new Date(now.getTime() + response.expires_in * 1000);
        authToken = { token: response.access_token, expires: expires };
        return authToken.token;
    }

    return authToken.token;
};
