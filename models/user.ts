import _crypto = require('../helpers/crypto')

export function userHash(userId: string): string { 
    return _crypto.sha256(userId) 
};

