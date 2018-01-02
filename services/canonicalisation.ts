// https://blog.tompawlak.org/calculate-checksum-hash-nodejs-javascript
import crypto = require('crypto');

interface RdostrId {
    Version: string,
    Urn: string,
    Id: string,
    UrnId: string
}

function checksum(data: string, algorithm: string = 'md5', encoding: crypto.HexBase64Latin1Encoding = 'hex'): string {
    return crypto
        .createHash(algorithm)
        .update(data, 'utf8')
        .digest(encoding);
}

export function getArtistId(artist: string): RdostrId {
    // trim and lowercase
    let token = artist.trim().toLowerCase();

    // format as urn
    // urn:rdostr:v1:artist/radiohead/album/ok computer/track/paranoid android
    let urn = `urn:rdostr:v1:artist/${token}`

    // hash
    let hash = checksum(urn);

    return {
        Version: "1",
        Urn: urn,
        Id: hash,
        UrnId: `urn:rdostr:v1:${hash}`
    };
}
