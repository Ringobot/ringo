"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
// https://blog.tompawlak.org/calculate-checksum-hash-nodejs-javascript
const crypto = require("crypto");
function checksum(data, algorithm = 'md5', encoding = 'hex') {
    return crypto
        .createHash(algorithm)
        .update(data, 'utf8')
        .digest(encoding);
}
function getArtistId(artist) {
    // trim and lowercase
    let token = artist.trim().toLowerCase();
    // format as urn
    // urn:rdostr:v1:artist/radiohead/album/ok computer/track/paranoid android
    let urn = `urn:rdostr:v1:artist/${token}`;
    // hash
    let hash = checksum(urn);
    return {
        Version: "1",
        Urn: urn,
        Id: hash,
        UrnId: `urn:rdostr:v1:${hash}`
    };
}
exports.getArtistId = getArtistId;
