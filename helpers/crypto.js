"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const crypto = require("crypto");
function ascii(input) {
    return Buffer.from(input).toString("ascii");
}
exports.ascii = ascii;
function sha1(input) {
    let sha = crypto.createHash('sha1');
    sha.update(input);
    return sha.digest('hex');
}
exports.sha1 = sha1;
function sha256(input) {
    let sha = crypto.createHash('sha256');
    sha.update(input);
    return sha.digest('hex');
}
exports.sha256 = sha256;
function randomString(length = 40) {
    // https://stackoverflow.com/a/14869745/610731
    return crypto.randomBytes(length / 2).toString('hex');
}
exports.randomString = randomString;
function base64(input) {
    return Buffer.from(input).toString('base64');
}
exports.base64 = base64;
function unBase64(input) {
    return Buffer.from(input, 'base64').toString('ascii');
}
exports.unBase64 = unBase64;
