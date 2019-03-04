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

1. ✅ `play` / `start` creates station 
1. ✅ Just `join` or `listen`
1. ✅ `@ringo sync`
1. ✅ `start` starts up at whatever playlist/album the user is currently playing
1. ✅ `@abrahamlarsen is no longer playing. Would you like to play Steve Spacek?` OR just play whatever
   played last
1. ✅ `start` alias for `play`
1. ✅ `@ringo play Johnny Cash #RIPJohnnyCash`
1. ✅ If no active device, click button to open player
   <https://open.spotify.com/track/6OxAnk3AroaW4c5hsUXNkB?si=BcWvVqgJS-6wE1B1cAFZEw&nd=1>
1. ✅ Backoff retry Spotify connections (getting 502's)
1. ~~DM auth, DM no active device~~ - DM is not possible.

#### dan/simple-stations

1. ✅ Join Hero Card
1. ✅ Only two types of stations, channel and user and they persist
1. ✅ When you `play` in a channel you are changing the station for the channel
1. ✅ When you `play` in DM you are changing your own channel
1. ✅ `join` in a channel joins the channel station
1. ▶ `join` in the DM joins any other station, e.g. `join #music_lovers` (joins music_lovers channel
   station) or `join @abraham` (joins another abraham's station)
1. play current artist, playlist or album. Otherwise warn.
1. Revert play "playlist name" feature?
1. OK for people to join your personal station (means they can see what you are playing at any time)
1. If owner stops playing Ringo searches for new owner
1. `@daniel has joined #general playing "Joy" with 3 others`
1. Separate Station Table? Station Type = Channel|User. 
1. New Listener table?
1. Station list?

#### dan/whkmas

1. `You and 20 others are playing #JoeGoddard`
1. `next | skip | skip 3`
1. Christian algorithm
1. Support Artist
1. Support Albums
1. 🔹 VISH DEMO
1. `You pasted a link, would you like to... (start a station | add to playlist | change the station | ...)`
1. Ignore anything before `@ringo`
1. Turn LUIS on for Skitch
1. Search results carousel

Sending a slack DM: <https://github.com/Microsoft/BotBuilder/issues/2923> <https://stackoverflow.com/questions/44353520/is-there-a-way-to-start-formflow-dialog-on-conversationupdate-event>

#### dan/sync

1. Once you join a channel station, your Spotify player follows the station
1. Try sideloading teams
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
1. `@ringo version`
1. `@ringo status`
1. `play album`
1. Fade and switch
1. `@ringo airhorn`
1. `@ringo skip 3`
1. `@ringo np`
1. `@ringo star`
1. support Shuffle
1. RULE: If @Daniel2 is now playing a different playlist (and no station), join and create station
1. `start #MerryChristmas`
1. What would you like to play? prompt
1. `Station #choicetraxbychromesparks is no longer playing. Would you like to Play "choice trax by chrome sparks"? Type "@ringo play spotify:user:jeremymalvin:playlist:0dPOIm0xYhVRMTqfJKRatQ" to start.`
1. Play buttons in place of "Would you like to play X"
1. `help`
1. `play #LetMeDJ` is an alias to `join #LetMeDJ`
1. `@ringo join @Daniel2`
1. join any other user to sync

### SpotifyApi.NetCore

1. Move models to `Models` namespace to avoid ambiguous collisions with client Models

## Station hash tags

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


## Links

<https://developer.spotify.com/documentation/general/guides/content-linking-guide/>

Using the Spotify Connect Web API: <https://developer.spotify.com/documentation/web-api/guides/using-connect-web-api/>