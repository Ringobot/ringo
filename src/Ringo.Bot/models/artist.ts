export interface Image {
    height: number,
    url: string,
    width: number
}

export interface Artist {
    name: string,
    spotify: {
        id: string,
        uri: string
    }
    images: Image[]
}

export function MapToArtist(spotifyArtist: any): Artist {
    if (!spotifyArtist) return null;
    if (!spotifyArtist.name && !spotifyArtist.id && !spotifyArtist.uri) {
        // it's not an artist
        throw new Error(`${spotifyArtist} is not a Spotify artist`)
    }

    return {
        name: spotifyArtist.name,
        spotify: {
            id: spotifyArtist.id,
            uri: spotifyArtist.uri
        },
        images: spotifyArtist.images
    };
}

export function MapToArtists(spotifyData: any): Artist[] {
    if (!spotifyData) return null;

    // spotify artist responses come in two shapes (!)

    let data = spotifyData.artists.items || spotifyData.artists

    if (!data){
        throw new Error(`${spotifyData} is not a valid Spotify artists response`);
    }

    return data.map(MapToArtist);
}