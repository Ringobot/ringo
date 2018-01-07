require('dotenv').config();
import crypto = require("crypto");

export function newGuid(): string {
    return crypto.randomBytes(16).toString("hex");
}
