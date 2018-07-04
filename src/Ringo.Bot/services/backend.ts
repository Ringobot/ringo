import * as _httpj from "./httpjson";

const baseUrl = 'https://ringofuncbackend.azurewebsites.net/api';

export async function searchArtists(artist: string, limit?: number) {
    try {

        // https://api.spotify.com/v1/search?q=radiohead&type=artist

        let response = await _httpj.get(`${baseUrl}/artist/search?code=${process.env.FunctionAuth}&artist=${encodeURIComponent(artist)}&limit=${limit}`);
        return response;
    } catch (e) {
        throw e;
    }

}

// export async function getPlaylists(username: string, offset: number) {
//     try {
//         let token = await _auth.getClientAuthToken();

//         let response = await _httpj.get(baseUrl + '/users/' + encodeURIComponent(username.toLowerCase()) + '/playlists?limit=50&offset='
//             + offset, { 'Authorization': 'Bearer ' + token });

//         return response;
//     } catch (e) {
//         throw e;
//     }

// }

// export async function getRecommendation(artistSeed: string, limit?: number) {
//     try {
//         let response = await _httpj.get(`${baseUrl}/artist/?code=${process.env.FunctionAuth}&artist=${artistSeed}&limit=${limit}`);

//         return response;
//     } catch (e) {
//         throw e;
//     }

// }

export async function getRelatedArtists(artistId: string) {
    // GET https://api.spotify.com/v1/artists/{id}/related-artists
    try {
        let response = await _httpj.get(`${baseUrl}/artist/related?code=${process.env.FunctionAuth}&artist=${artistId}`);

        return response;
    } catch (e) {
        throw e;
    }
};

export async function getArtist(artistId: string) {
    // GET https://api.spotify.com/v1/artists/{id}

    try {
        let response = await _httpj.get(`${baseUrl}/artist/id?code=${process.env.FunctionAuth}&artist=${artistId}`);

        return response;
    } catch (e) {
        throw e;
    }
};