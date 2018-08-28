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

export function mapToArtist(spotifyArtist: any): Artist {
    if (!spotifyArtist) return null;
    if (!(spotifyArtist.name && spotifyArtist.spotify.id && spotifyArtist.spotify.uri)) {
        // it's not an artist
        throw new Error(`${spotifyArtist} is not a Spotify artist`)
    }

    return {
        name: spotifyArtist.name,
        spotify: {
            id: spotifyArtist.spotify.id,
            uri: spotifyArtist.spotify.uri
        },
        images: spotifyArtist.images
    };
}

export function mapToArtists(spotifyData: any): Artist[] {
    if (!spotifyData) return null;

    // spotify artist responses come in two shapes (!)

    let data = spotifyData.artists.items || spotifyData.artists

    if (!data){
        throw new Error(`${spotifyData} is not a valid Spotify artists response`);
    }

    return data.map(mapToArtist);
}

export function mapGraphToArtists(data:any[]): Artist[]{
    let artists:Artist[] = []
    
    data.forEach(item => {
        artists.push({
            name: item.properties['name'][0].value,
            spotify: {
                id: item.properties['spotifyId'] && item.properties['spotifyId'][0].value,
                uri: item.properties['spotifyUri'] && item.properties['spotifyUri'][0].value
            },
            images: [{
                height: undefined,
                url: item.properties['imageUrl'] && item.properties['imageUrl'][0].value,
                width: undefined
            }]
        })
    });

    return artists;
}
