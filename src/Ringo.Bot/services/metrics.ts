import appInsights = require('applicationinsights');

export function start() {
    appInsights.setup();
    appInsights.start();
}

export function setAuthenticatedUserContext(sessionId:string, userHash:string) {
    try {
        console.log(`metrics:setAuthenticatedUserContext: sessionId=${sessionId}, userHash=${userHash}`);
        let client = appInsights.defaultClient;
        client.context.tags[client.context.keys.sessionId] = sessionId;
        client.context.tags[client.context.keys.userId] = userHash;
        client.context.tags[client.context.keys.userAuthUserId] = userHash;

    } catch (e) {
        // log and continue
        console.error(e);
    }
}

export function trackEvent(name: string) {
    try {
        console.log(`metrics:trackEvent:${name}`);
        appInsights.defaultClient.trackEvent({ name: name });
    } catch (e) {
        // log and continue
        console.error(e);
    }
}

export function trackError(error: any) {
    try {
        appInsights.defaultClient.trackException({ exception: error });
    } catch (e) {
        // log and continue
        console.error(e);
    }
}
