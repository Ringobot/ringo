"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const helpers = require("../helpers");
const chai_1 = require("chai");
require("mocha");
describe('group mentioned conversation data', () => {
    it('isGroup and isMentioned returns true', () => {
        let result = helpers.isGroup(message) && helpers.isMentioned(message);
        chai_1.expect(result).to.be.true;
    });
    it('isGroup and not isMentioned returns false', () => {
        let result = helpers.isGroup(message) && !helpers.isMentioned(message);
        chai_1.expect(result).to.be.false;
    });
});
describe('getEntityText', () => {
    it('returns entity text', () => {
        let result = helpers.getEntityText(message2, entity);
        chai_1.expect(result).to.eq("spotify:artist:25IG9fa7cbdmCIy3OnuH57");
    });
});
let entity = {
    endIndex: 43,
    entity: "spotify:artist:25ig9fa7cbdmciy3onuh57",
    startIndex: 7,
    type: "spotifyUri",
};
let message2 = {
    "type": "message",
    "timestamp": "2018-04-03T09:19:58.8655932Z",
    "text": "I like spotify:artist:25IG9fa7cbdmCIy3OnuH57",
    "attachments": [],
    "entities": [],
    "sourceEvent": {
        "SlackMessage": {
            "token": "arQmtvSCH2DysMpC2tAofQC0",
            "team_id": "TA0VBN61L",
            "api_app_id": "A9ZJ8EECR",
            "event": {
                "type": "message",
                "user": "U9ZDS52G0",
                "text": "<@UA09044MT> I like massive attack",
                "ts": "1522747197.000185",
                "channel": "C9ZTZH31A",
                "event_ts": "1522747197.000185"
            },
            "type": "event_callback",
            "event_id": "EvA1862V1D",
            "event_time": 1522747197,
            "authed_users": [
                "UA09044MT"
            ]
        },
        "ApiToken": "xoxb-340306140741-rsNvhoyfSmFmvXazBVMShMUA"
    },
    "address": {
        "id": "f3dc2064f5f54eaf99b3e0722bb2efa5",
        "channelId": "slack",
        "user": {
            "id": "U9ZDS52G0:TA0VBN61L",
            "name": "daniel"
        },
        "conversation": {
            "isGroup": true,
            "id": "B9ZDXALCQ:TA0VBN61L:C9ZTZH31A",
            "name": "testing2"
        },
        "bot": {
            "id": "B9ZDXALCQ:TA0VBN61L",
            "name": "ringo"
        },
        "serviceUrl": "https://slack.botframework.com"
    },
    "source": "slack",
    "agent": "botbuilder",
    "user": {
        "id": "U9ZDS52G0:TA0VBN61L",
        "name": "daniel"
    }
};
let message = {
    "type": "message",
    "timestamp": "2018-04-03T09:19:58.8655932Z",
    "text": "@ringo I like massive attack",
    "attachments": [],
    "entities": [
        {
            "mentioned": {
                "id": "B9ZDXALCQ:TA0VBN61L",
                "name": "ringo"
            },
            "text": "@ringo",
            "type": "mention"
        }
    ],
    "sourceEvent": {
        "SlackMessage": {
            "token": "arQmtvSCH2DysMpC2tAofQC0",
            "team_id": "TA0VBN61L",
            "api_app_id": "A9ZJ8EECR",
            "event": {
                "type": "message",
                "user": "U9ZDS52G0",
                "text": "<@UA09044MT> I like massive attack",
                "ts": "1522747197.000185",
                "channel": "C9ZTZH31A",
                "event_ts": "1522747197.000185"
            },
            "type": "event_callback",
            "event_id": "EvA1862V1D",
            "event_time": 1522747197,
            "authed_users": [
                "UA09044MT"
            ]
        },
        "ApiToken": "xoxb-340306140741-rsNvhoyfSmFmvXazBVMShMUA"
    },
    "address": {
        "id": "f3dc2064f5f54eaf99b3e0722bb2efa5",
        "channelId": "slack",
        "user": {
            "id": "U9ZDS52G0:TA0VBN61L",
            "name": "daniel"
        },
        "conversation": {
            "isGroup": true,
            "id": "B9ZDXALCQ:TA0VBN61L:C9ZTZH31A",
            "name": "testing2"
        },
        "bot": {
            "id": "B9ZDXALCQ:TA0VBN61L",
            "name": "ringo"
        },
        "serviceUrl": "https://slack.botframework.com"
    },
    "source": "slack",
    "agent": "botbuilder",
    "user": {
        "id": "U9ZDS52G0:TA0VBN61L",
        "name": "daniel"
    }
};
