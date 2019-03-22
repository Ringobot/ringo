# Ringo bot .NET Core

## TODO

### dan/auth-bugs

1. ~~BUG: Expired token on join bug~~
1. ~~Don't authorize in the clear! - still not fixed, Play will start auth flow in group chat :|~~
1. ~~BUG: Wrong user name in join message: `@abrahamlarsen has joined @daniel playing "No Other"! Tell your friends to type "join @abrahamlarsen" into Ringobot to join the party!`~~
1. ~~`authorize reset`~~
1. ~~`auth` alias~~
1. ~~`auth (magic number)`~~

### dan/easy-join

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

### dan/simple-stations

1. ✅ Join Hero Card
1. ✅ Only two types of stations, channel and user and they persist
1. ✅ When you `play` in a channel you are changing the station for the channel
1. ✅ When you `play` in DM you are changing your own channel
1. ✅ `join` in a channel joins the channel station
1. ✅ `join` in the DM joins any other station, e.g. `join #music_lovers` (joins music_lovers channel
   station) or `join @abraham` (joins another abraham's station)
1. ✅ ⭐ play current ~~artist~~, playlist or album. Otherwise warn.
1. OK for people to join your personal station (means they can see what you are playing at any time)
1. ~~Revert play "playlist name" feature?~~

### dan/whkmas

1. ⭐ Christian algorithm
1. ⭐ `next | skip | skip 3`
1. ⭐ `You and 20 others are playing #JoeGoddard`
1. ⭐ `You pasted a link, would you like to... (start a station | add to playlist | change the station | ...)`
1. ⭐ `You mentioned Album x, would you like Ringo to add it to a playlist? Add to current playlist a | Add to channel playlist b | Add to A different playlist `
1. Ignore anything before `@ringo`
1. Turn LUIS on for Skitch
1. Search results carousel
1. ⭐ `@ringo np`
1. ⭐ `@ringo star`
1. ⭐ Turn off shuffle
1. @skitch: So this is spotify's radio - how would we do like a day of each others radio stations e.g.
   say we make friday's 'join' days  and take turns at doing morning set, miday set and jamming 
   afternoon set? Would we just use playlists?
1. `queue Trojan Dub` - adds playlist to station queue 
1. `queue 0600 Dan's breakfast show` - queues playlist to play at 6am
1. ⭐ Mobile friendly magic number page: https://blog.teamtreehouse.com/create-an-absolute-basic-mobile-css-responsive-navigation-menu.
   Click to copy. Redirect back to app via slack:XXXX URI?
1. Should there only be one station per channel? What if two users want to play different lists in the
   same channel at the same time? 🤔
1. Check for shuffle flag
1. 🐞 Two spaces between `@ringo` and `play`
1. 🐞 sync when no station = error

### dan/endless-stations

1. ⭐ If owner stops playing Ringo searches for new owner
1. ⭐ `@daniel has joined #general playing "Joy" with 3 others`
1. Separate Station Table? Station Type = Channel|User. 
1. New Listener table?
1. Station list?

### dan/sync

1. Once you join a channel station, your Spotify player follows the station
1. ~~Auth via Spotify App?~~ Only in Mobile SDK
1. Better Playlist sorting and selection (carousel)
1. To sync or not sync
1. live mode / discover mode (Abraham)
1. Sync at the end of each song?
1. Automatically switch playlists if the person you are listening to changes?

### dan/slack-mvp-release

1. Application Insights
1. Azure Devops Staging pipeline
1. `ringo privacy`
1. `ringo feedback`

### dan/teams

1. Try sideloading teams
1. Teams enhancements

### dan/twitter

1. Twitter bot

### dan/skype

1. Skype features

### Unplanned

1. Support albums
1. Port Ringo JS features
1. List devices and choose one
1. Speech
1. Sync playlists
1. `@ringo version`
1. `@ringo status`
1. Fade and switch
1. `@ringo airhorn`
1. support Shuffle
1. ✅ RULE: If @Daniel2 is now playing a different playlist (and no station), join and create station
1. ✅ `start #MerryChristmas`
1. What would you like to play? prompt
1. ✅ Play buttons in place of "Would you like to play X"
1. `help`
1. `play #LetMeDJ` is an alias to `join #LetMeDJ` ?
1. ✅ `@ringo join @Daniel2`
1. ✅ join any other user to sync
1. Ringo account creates and owns curated playlists (based on graph recommendations) and suggests them
   first up to get users started (Matt)

### SpotifyApi.NetCore

1. Move models to `Models` namespace to avoid ambiguous collisions with client Models

## Beta users

* Whkmas
* Vishesh
* Akudos
* MSFT NZ (Teams)

## Cristian algorithm

Cristian's algorithm works between a process P, and a time server S — connected to a source of UTC 
(Coordinated Universal Time). Put simply:

    P requests the time from S
    After receiving the request from P, S prepares a response and appends the time T from its own clock.
    P then sets its time to be T + RTT/2

This method assumes that the RTT (Round Trip Time) is split equally between request and response, which 
may not always be the case but is a reasonable assumption on a LAN connection.

Further accuracy can be gained by making multiple requests to S and using the response with the shortest 
RTT. 

## Ringo Station URI

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