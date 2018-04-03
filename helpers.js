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
