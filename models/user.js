"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const _crypto = require("../helpers/crypto");
function userHash(userId) {
    return _crypto.sha256(userId);
}
exports.userHash = userHash;
;
