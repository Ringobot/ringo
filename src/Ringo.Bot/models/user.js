"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const _crypto = require("../helpers/crypto");
function userHash(session) {
    let id = userId(session);
    return id ? _crypto.sha256(id) : null;
}
exports.userHash = userHash;
;
function userId(session) {
    let userId = ((session && session.user && session.user.id)
        || (session && session.message && session.message.user && session.message.user.id));
    if (!userId) {
        console.warn('Could not find user Id in Session');
        return null;
    }
    return userId;
}
exports.userId = userId;
