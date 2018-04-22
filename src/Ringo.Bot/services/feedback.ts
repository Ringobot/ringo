import _servicebus = require('./servicebus');

export async function sendFeedback(userId, sessionId, message){
    try {
        await _servicebus.sendMessage('feedback', {
            userHash: userId,
            sessionHash: sessionId,
            message: message
        });
    } catch (e){
        throw e;
    }
}
