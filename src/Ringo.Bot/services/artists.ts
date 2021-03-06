import artist = require('../models/artist');
import entityrelation = require('../models/entityrelation');
import _canonical = require('./canonicalisation');
import _graph = require('./graphstorage');
import * as _httpj from "./httpjson";
import _backend = require('./backend');


export async function searchArtists(artistName: string, limit?: number): Promise<artist.Artist[]> {
    return await _backend.searchArtists(artistName, limit);
}

export async function getRelatedArtists(artistId: string): Promise<artist.Artist[]> {
    return await _backend.getRelatedArtists(artistId);
};

export function getArtist(artistId: string) {
    return _backend.getArtist(artistId);
};

export async function getArtistByUri(uri: string): Promise<artist.Artist> {
    let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/

    if (!spotifyArtistRegex.test(uri)) {
        throw new Error('Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported');
    }

    let artistId = uri.split(":")[2];

    try {
        return await artist.mapToArtist(await _backend.getArtist(artistId));
    } catch (e) {
        throw e;
    }
}

export async function pushRelatedArtist(baseArtist: artist.Artist, relatedArtistList: artist.Artist[]) {

    try {
        let erList: entityrelation.EntityRelationship[] = [];

        relatedArtistList.forEach(async Artist => {
            let baseArtistId = `${baseArtist.name.toLowerCase()}:${_canonical.getArtistId(baseArtist.name).Id}`;
            let relatedArtistId = `${Artist.name.toLowerCase()}:${_canonical.getArtistId(Artist.name).Id}`;

            var er = {
                FromVertex:
                {
                    Id: baseArtistId,
                    Name: baseArtist.name,
                    Properties: {
                        type: 'artist',
                        spotifyid: Artist.spotify.id,
                        spotifyuri: Artist.spotify.uri,
                        images: Artist.images[0].url
                    }
                },
                ToVertex:
                {
                    Id: relatedArtistId,
                    Name: Artist.name,
                    Properties: {
                        type: 'artist',
                        spotifyid: Artist.spotify.id,
                        spotifyuri: Artist.spotify.uri,
                        images: Artist.images[0].url
                    }

                },
                Relationship: 'related',
                RelationshipDate: new Date()
            }
            erList.push(er);
        });

        await postRelated(erList)

    } catch (e) {
        throw e;
    }
}

/**
 * Get all of the artists that a User likes
 * @param userId 
 */

export async function artistsUserLikes(userId): Promise<artist.Artist[]> {
    let data = await _graph.vertexEdgeVertices(userId, 'likes', 'artist')
    return artist.mapGraphToArtists(data);
}

export async function userLikesArtist(userId: string, artist: artist.Artist) {
    if (!userId) throw new Error('user cannot be null');

    // graph artist Id
    let artistId = `${artist.name.toLowerCase()}:${_canonical.getArtistId(artist.name).Id}`;

    let erList: entityrelation.EntityRelationship[] = [];

    try {
        var entityRelationship = {
            FromVertex: {
                Id: userId,
                Name: userId,
                Properties: {
                    type: "user"
                }
            },
            ToVertex: {
                Id: artistId,
                Name: artist.name,
                Properties: {
                    type: "artist",
                    spotifyid: artist.spotify.id,
                    spotifyuri: artist.spotify.uri,
                    imageurl: artist.images[0].url
                }
            },
            Relationship: "likes",
            RelationshipDate: new Date()
        }

        erList.push(entityRelationship)

        await postRelated(erList)
        
    } catch (e) {
        throw e;
    }
};


export async function postRelated(erList) {
    try {
        let url = process.env.API_BACKEND + '/RelatedArtists_HttpStart';
        await _httpj.post(url, JSON.stringify(erList), { 'Content-Type': 'application/json' });
    } catch (e) {
        throw e;
    }
}


/**
 * Finds true and the artist if only one artist (with an image) is found in the array
 * @param artists an array of Artists (returned by a search API)
 */
export function findMatch(artists: artist.Artist[]): [boolean, artist.Artist] {
    let artistsWithImages = artists.filter(i => i.images.length > 0);
    return artistsWithImages.length === 1 ? [true, artistsWithImages[0]] : [false, null];
}
