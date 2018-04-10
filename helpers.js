"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
function isGroup(message) {
    return message.address.conversation && message.address.conversation.isGroup;
}
exports.isGroup = isGroup;
function isMentioned(message) {
    return message.entities
        && message.entities.filter(e => e.type == 'mention').find(e => e.mentioned.id == message.address.bot.id) !== undefined;
}
exports.isMentioned = isMentioned;
function getEntityText(message, entity) {
    // https://github.com/Microsoft/BotBuilder/issues/963
    return message.text.substr(entity.startIndex, entity.endIndex - entity.startIndex + 1);
}
exports.getEntityText = getEntityText;
function findMatch(artists) {
    let result = { matched: false, artist: null };
    let artistsWithImages = artists.filter(i => i.images.length > 0);
    result.matched = artistsWithImages.length === 1;
    if (result.matched)
        result.artist = artistsWithImages[0];
    return result;
}
exports.findMatch = findMatch;
