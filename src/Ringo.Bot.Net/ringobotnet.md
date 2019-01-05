# Ringobotnet dialogs

## Teams
    /play playlist Prince
        12,768 playlists found. 
        Daniel is playing "This is Prince"
    /join Daniel
        Someguy's iPhone is playing "This is Prince" with Daniel

    /play playlist Prince
        12,768 playlists found.
        Playing "This is Prince" in "NZ ISV Team Chat"
    /join
        Someguy's iPhone is playing "This is Prince" with "NZ ISV Team Chat"

    1. Get channel $playlist from state
    2. GET me/player        # get the master user's current track
    3. Set volume zero? Pause?
    4. PUT me/player/play   # start playback of $playlist with an offset to the current track
    5. GET me/player        # get master user's progress
    6. PUT me/player/seek   # seek to the position in the track

## Twitter!

    @ringobot play playlist Prince #RIPPrince
        @DanielLarsenNZ 12,768 playlists found. Playing "This is Prince" #RIPPrince

    @ringobot play @DanielLarsenNZ
        @SomeguyNZ is playing "This is Prince" #RIPPrince with 
          @DanielLarsenNZ and 262 others.

    @ringobot play #RIPPrince
        @SomeguyNZ is playing "This is Prince" #RIPPrince with 
          @DanielLarsenNZ and 19,678 others.

    @ringobot play podcast This American Life
        @SomeguyNZ is playing "This American Life" with 
            @Alice and 201 others

    @ringobot who
        @DanielLarsenNZ is playing playlist "This is Prince" #RIPPrince 
            with 19,678 others
        @Alice is playing podcast "The Daily" with 12,456 others
        @Bob is playing playlist "Best of 2018" with 8,686 others
        ... (10 results)
    
    @ringobot who page 2
        (next 10 results)

## Instagram!!