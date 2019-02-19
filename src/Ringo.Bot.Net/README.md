# Ringo bot .NET Core

## TODO

#### dan/auth-bugs

1. ~~BUG: Expired token on join bug~~
1. ~~Don't authorize in the clear! - still not fixed, Play will start auth flow in group chat :|~~
1. ~~BUG: Wrong user name in join message: `@abrahamlarsen has joined @daniel playing "No Other"! Tell your friends to type "join @abrahamlarsen" into Ringobot to join the party!`~~
1. ~~`authorize reset`~~
1. ~~`auth` alias~~
1. ~~`auth (magic number)`~~

#### dan/easy-join

1. `play` / `start` creates station ✅
1. Just `join` or `listen` ✅
1. Play buttons in place of "Would you like to play X"
1. `start` starts up at whatever playlist/album the user is currently playing
1. If no active device, click button to start player
1. `@abrahamlarsen is no longer playing. Would you like to play Steve Spacek?` OR just play whatever
   played last ✅
1. `play album`
1. `start` alias for `play` ✅
1. `start #MerryChristmas`
1. join any other user to sync
1. What would you like to play? prompt

Station hash tags

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

#### dan/dm-auth

1. DM auth

Sending a slack DM: <https://github.com/Microsoft/BotBuilder/issues/2923> <https://stackoverflow.com/questions/44353520/is-there-a-way-to-start-formflow-dialog-on-conversationupdate-event>

#### dan/sync

1. Try sideloading teams
1. Christian algorithm
1. sync
1. Mobile friendly magic number page: https://blog.teamtreehouse.com/create-an-absolute-basic-mobile-css-responsive-navigation-menu.
   Click to copy. 
1. Auth via Spotify App?
1. Should be able to join anyone - Ringo figures out which station
1. "You and n other listeners"
1. Play an Album
1. Better Playlist sorting and selection (carousel)
1. Show Album hero artwork
1. Application insights
1. Azure Devops Staging pipeline
1. Launch app to make active?

### Submit to Slack marketplace

1. Join a station from any channel (hashcode)
1. Teams enhancements

### Submit to Teams marketplace

1. Twitter bot

### Soft launch on Twitter

1. Automatically switch playlists if the person you are listening to changes
1. Port Ringo JS features
1. List devices and choose one
1. To sync or not sync
1. live mode / discover mode
1. Sync at the end of each song?
1. Ringo like this song
1. Ringo add this song to playlist x
1. Speech
1. Skype features

### Submit to Skype marketplace

### Unplanned

1. Sync playlists
1. Return a list of stations for the TeamChannel

### SpotifyApi.NetCore

1. Move models to `Models` namespace to avoid ambiguous collisions with client Models