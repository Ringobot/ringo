"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function MapToArtist(spotifyArtist) {
    if (!spotifyArtist)
        return null;
    return {
        name: spotifyArtist.name,
        spotifyId: spotifyArtist.id
    };
}
exports.MapToArtist = MapToArtist;
