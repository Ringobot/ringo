# Data modeling

* C1. Create a `User`
* C2. Create a `Station`
* Q1. Get `User`
* Q2. Get `Station`
* C3. Increment/Decrement `Station` `Listener` count 
* C4. Create new `Listener` for a Station
* C5. Get all `Listeners` for a `Station`
* C6. Delete a `Listener` that is no longer playing the `Station`
* CX. Create a `Programme` (Queue of Playlists)
* Q3. Get the Owner Listener
* Q4. Get a recently modified active Listener
* C7. Change the Owner
* C8. Delete the `Station`
* Q5. Get the token for a `Listener` -> `User`
* Q6. Get `Station` ListenerCount
* Q7. Get `Station` by Hashtag
* Q8. Get all Stations for a Channel, ordered by Listeners (desc), Created (desc) 

### XXX

    // The following characters are restricted and cannot be used in the Id 
    // property: '/', '\\', '?', '#'

    // User
    {
        Id : sha(channelId, userId),
        PK: (User.Id),
        Type: "user",
        UserId,
        ChannelId,
        Username,
        Auth : {
            BearerAccessToken : {},
            CreatedDate: datetime,
            State : string,
            ValidatedDate: datetime?
            Validated: (ValidatedDate.HasValue)
        },
        CreatedDate: datetime
    }

    // Station
    {
        PK: Station.Id,
        Id: Station.Uri,
        Uri: (uri())
        Type: "station",
        Name,
        Hashtag,
        Playlist: { Playlist },
        Album: { Album },
        Owner: { User },
        ListenerCount: int,
        CreatedDate: datetime,
        IsActive: bool,
        ActiveListeners: [ { Listener (max 10) } ]
    }

    // Listener
    {
        PK: Station.Uri,
        Id: User.Id,
        Type: "listener",
        Station: { Station },
        User: { User },
        CreatedDate,
        LastActiveDate: datetime?
    }
