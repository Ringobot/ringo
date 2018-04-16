import crypto = require('crypto');

export function ascii(input: string): string {
    return Buffer.from(input).toString("ascii");
}

export function sha1(input: string) {
    let sha = crypto.createHash('sha1');
    sha.update(input);
    return sha.digest('hex');
}

export function sha256(input: string) {
    let sha = crypto.createHash('sha256');
    sha.update(input);
    return sha.digest('hex');
}

export function randomString(length: number = 40) {
    // https://stackoverflow.com/a/14869745/610731
    return crypto.randomBytes(length / 2).toString('hex');
}

export function base64(input: string): string {
    return Buffer.from(input).toString('base64');
}

export function unBase64(input: string): string {
    return Buffer.from(input, 'base64').toString('ascii');
}
