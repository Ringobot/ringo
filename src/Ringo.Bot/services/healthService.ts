import _table = require('./tablestorage')

export async function getHealth(req, res, next) {
    // version number from package.json
    var packageJson = require('../package.json')

    // default result
    let result = {
        Status: 'Up',
        Version: packageJson.version,
        DateTime: new Date(),
        Services:[]
    }

    // table storage
    try {
        // use a high-res timer
        let start = process.hrtime()
        // get top 1 from a table TODO: Health table
        await _table.get('UserSpotifyAuth', _table.createQuery().top(1))
        let elapsed = process.hrtime(start);
        // calculate elapsed milliseconds: TODO: helper function
        let elapsedMs = (elapsed[0] * 1000) + (elapsed[1] / 1000000)

        // Business logic for deciding if Table Storage is Up, Trouble or Down
        let health = 
            // <100ms = Up
            elapsedMs < 100 ? 'Up' : 
            // < 5 seconds = Trouble
            elapsedMs < 3000 ? 'Trouble' : 
            // > 5 seconds = Down
            'Down'

        result.Services.push({
            Name: "Table Storage",
            Status: health,
            Elapsed: elapsedMs
        })

    } catch (e) {
        // error means Down
        console.error(e)
        result.Services.push({
            Name: "Table Storage",
            Status: "Down",
            Message: "Error",
            HealthScore: 0.0
        })
    }

    // Status reflects table storage status (v1). TODO: Combined health of services
    result.Status = result.Services[0].Status

    res.send(result)
    return next()
}