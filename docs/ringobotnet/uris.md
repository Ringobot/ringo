# URIs, Ids, UIds

* **Channel**: Bot channel, i.e. Teams, Slack, Twitter
    * **ChannelId**: Identifies a channel, but not the tenant, e.g. "msteams", "slack", "skype"
* **Tenant**: Combination of a ChannelId and a unique identifier for the tenant (if any).
* **Conversation**: Any conversation between the bot and another user. Can be group or non-group conversation
    as indicated by the `isGroup` flag.
* **User**: Ringobot's own User has a 1:1 relationship with a Channel's User.
    * **UserId**: The Channel's UserId.

## Ringo Station URI

### v4

    // The following characters are restricted and cannot be used in the Id 
    // property: '/', '\\', '?', '#'

    // User.Id
    ringo:{channel_id}:{channel_team_id.ToLower()}:user:user_id.ToLower()}
    ringo:slack:ta0vbn61l:user:abc123def456
    
    // Station.Id (Conversation)
    ringo:{channel_id}:{channel_team_id.ToLower()}:station:conversation:{lower_word(conversation_name)}:hashtag:{lower_word(hashtag)}
    ringo:slack:ta0vbn61l:station:conversation:musiclovers:hashtag:loveplaydance8scenesfromthefloor
    ringo:skype::station:conversation:musiclovers:hashtag:loveplaydance8scenesfromthefloor
    
    // Station.Id (User)
    ringo:{channel_id}:{channel_team_id.ToLower()}:station:user:{lower_word(user_name)}
    ringo:slack:ta0vbn61l:station:user:daniel

    // Station.PK 
    ringo:{base64(tenant)}:station:conversation:{lower_word(conversation_name)}
    ringo:{base64(tenant)}:station:user:{lower_word(user_name)}
    ringo:c2xhY2svVEEwVkJONjFM:station:conversation:musiclovers
    ringo:c2xhY2svVEEwVkJONjFM:station:user:daniel

### v3

    // user station
    ringo:{tenant62}:station:user:{username62}[:hashtag:{hashtag}]

    // "channel" station
    ringo:{tenant62}:station:channel:{channelName62}[:hashtag:{hashtag}]

### v2

    ringo:slack/TA0VBN61L:station:channel/testing3
    ringo:msteams/def9374638:station:channel/testing4
    ringo:twitter:station:hashtag/datenight

    ringo:slack/TA0VBN61L:station:user/daniel312
    ringo:msteams/def9374638:station:user/abcdef12452

### v1

    # These could all refer to the same station
    # Stations are global to a realm
    # Realms are Slack/Team, Teams/Team, Twitter, Skype/User

    channelId.lower()/team_id/@username.lower()
    slack/TA0VBN61L/@daniel

    channelId.lowr()/team_id/#conversation.name.replace(\W).lower()/SlackMessage.event.channel
    slack/TA0VBN61L/#testing3/CFX3U3TCJ

    channelId.lower()/team_id/#hashtag.lower()
    slack/TA0VBN61L/#heatwave

    channelId.lower()/team_id/#playlist_name.replace(\W).lower()
    slack/TA0VBN61L/#heatwave2019


    # StationUri
    {
        id: (base64(uri)),
        pk: (hash(uri)),
        uri: uri,
        channelUserId: channelUserId,
        stationId: stationId,
        hashtag: (hashtag)
    }

    # ChannelUser.Station
    {
        id: (guid),
        playlist: [playlist],
        created: (utc),
        modified: (utc),
        isActive: (bool),
        listenerCount: (int)
    }
