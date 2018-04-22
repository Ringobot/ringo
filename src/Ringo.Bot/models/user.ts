import _crypto = require('../helpers/crypto')

export function userHash(session: any): string {
    let id = userId(session);
    return id ? _crypto.sha256(id) : null;
};

export function userId(session: any): string {
    let userId = ((session && session.user && session.user.id)
        || (session && session.message && session.message.user && session.message.user.id));
    if (!userId) {
        console.warn('Could not find user Id in Session');
        return null;
    }
    return userId;
}
