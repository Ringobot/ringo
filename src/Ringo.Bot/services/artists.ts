import _spotify = require('./spotify');
import artist = require('../models/artist');
import entityrelation = require('../models/entityrelation');
import _servicebus = require('./servicebus');
import { GremlinClient } from '../gremlin/bundle';
import _canonical = require('./canonicalisation');

export async function searchArtists(artistName: string, limit?: number): Promise<artist.Artist[]> {
    return artist.mapToArtists(await _spotify.searchArtists(artistName, limit));
}

export async function getRelatedArtists(artistId: string): Promise<artist.Artist[]> {
    return artist.mapToArtists(await _spotify.getRelatedArtists(artistId));
};

export function getArtist(artistId: string) {
    return _spotify.getArtist(artistId);
};

export async function getArtistByUri(uri: string): Promise<artist.Artist> {
    let spotifyArtistRegex = /spotify:artist:[A-Za-z0-9]{22}/

    if (!spotifyArtistRegex.test(uri)) {
        throw new Error('Only Spotify URIs in the format spotify:artist:[A-Za-z0-9]{22} are currently supported');
    }

    let artistId = uri.split(":")[2];

    try {
        return await artist.mapToArtist(await _spotify.getArtist(artistId));
    } catch (e) {
        throw e;
    }
}

export async function pushRelatedArtist(baseArtist: artist.Artist, relatedArtistList: artist.Artist[]) {
    
    try {
        relatedArtistList.forEach(async Artist => {
            let baseArtistId = `${baseArtist.name.toLowerCase()}:${_canonical.getArtistId(baseArtist.name).Id}`;
            let relatedArtistId = `${Artist.name.toLowerCase()}:${_canonical.getArtistId(Artist.name).Id}`;
            // let er:entityrelation.EntityRelationship = {
        var er = {
                FromVertex:
                    {
                        Id: baseArtistId,
                        Name: baseArtist.name,
                        Properties: {
                            type: 'artist'
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

            await _servicebus.sendMessage('graph', er)
        });
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
