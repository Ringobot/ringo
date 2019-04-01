# Data modeling

* C1. Create a `User`
* C2. Create a `Station`
* Q2. Get `User`
* Q3. Get `Station`
* C4. Increment/Decrement `Station` `Listener` count 
* C5. Create new `Listener` for a Station
* Q6. Get all `Listeners` for a `Station`
* Q7. Delete a `Listener` that is no longer playing the `Station`
* C7. Create a `Programme` (Queue of Playlists)
* C8. Change the Station Owner (find the next owner)
* C9. Delete the `Station`
* C9. Get the token for a `Listener` -> `User`
* Q8. Get `Station` ListenerCount
* Q9. Get `Station` by Hashtag

### XXX

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
        Playlist: {},
        Album: {},
        ListenerCount: int,
        CreatedDate: datetime,
        IsActive: bool
    }

    // Listener
    {
        PK: StationId,
        Type: "listener",
        StationId,
        UserId,
        ListenerNumber : (sequential),
        IsOwner : bool
    }
