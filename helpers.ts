import builder = require('botbuilder');

export function isGroup(message: builder.IMessage) {
    return message.address.conversation && message.address.conversation.isGroup;
}

export function isMentioned(message: builder.IMessage) {
    return message.entities
        && message.entities.filter(e => e.type == 'mention').find(e => e.mentioned.id == message.address.bot.id) !== undefined;
}

export function getEntityText(message: builder.IMessage, entity: builder.IEntity) {
    // https://github.com/Microsoft/BotBuilder/issues/963
    return message.text.substr(entity.startIndex, entity.endIndex - entity.startIndex + 1);
}

export function findMatch(artists: any) {
    let result = {matched: false, artist: null};
    
    let artistsWithImages = artists.artists.items.filter(i => i.images.length > 0);
    result.matched = artistsWithImages.length === 1;
    
    if (result.matched) result.artist = artistsWithImages[0];

    return result;
}
