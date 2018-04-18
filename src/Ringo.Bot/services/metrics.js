"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const appInsights = require("applicationinsights");
function start() {
    appInsights.setup();
    appInsights.start();
}
exports.start = start;
function setAuthenticatedUserContext(sessionId, userHash) {
    try {
        console.log(`metrics:setAuthenticatedUserContext: sessionId=${sessionId}, userHash=${userHash}`);
        let client = appInsights.defaultClient;
        client.context.tags[client.context.keys.sessionId] = sessionId;
        client.context.tags[client.context.keys.userId] = userHash;
        client.context.tags[client.context.keys.userAccountId] = 'ringo';
        client.context.tags[client.context.keys.userAuthUserId] = userHash;
    }
    catch (e) {
        // log and continue
        console.error(e);
    }
}
exports.setAuthenticatedUserContext = setAuthenticatedUserContext;
function trackEvent(name) {
    try {
        console.log(`metrics:trackEvent:${name}`);
        appInsights.defaultClient.trackEvent({ name: name });
    }
    catch (e) {
        // log and continue
        console.error(e);
    }
}
exports.trackEvent = trackEvent;
function trackError(error) {
    try {
        appInsights.defaultClient.trackException({ exception: error });
    }
    catch (e) {
        // log and continue
        console.error(e);
    }
}
exports.trackError = trackError;
