"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require('dotenv').config();
const crypto = require("crypto");
function newGuid() {
    return crypto.randomBytes(16).toString("hex");
}
exports.newGuid = newGuid;
