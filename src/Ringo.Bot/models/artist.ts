export interface Artist {
    name:string,
    spotifyId:string
}

export function MapToArtist (spotifyArtist:any):Artist{
    if (!spotifyArtist) return null;
    return {
        name: spotifyArtist.name,
        spotifyId: spotifyArtist.id
    };
}
