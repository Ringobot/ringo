import _table = require('./tablestorage')

export async function getHealth(req, res, next) {
    // version
    var packageJson = require('../package.json')

    let result = {
        Status: 'Up',
        Health: 1.0,
        Version: packageJson.version,
        DateTime: new Date(),
        Services:[]
    }

    // table storage
    try {
        let start = process.hrtime()
        let tableHealth = await _table.get('UserSpotifyAuth', _table.createQuery().top(1))
        let elapsed = process.hrtime(start);
        let elapsedMs = (elapsed[0] * 1000) + (elapsed[1] / 1000000)

        let health = elapsedMs < 100 ? ['Up', 1.0] : elapsedMs < 500 ? ['Trouble', 0.5] : ['Down', 0.1]

        result.Services.push({
            Name: "Table Storage",
            Status: health[0],
            Health: health[1],
            Elapsed: elapsedMs
        })

    } catch (e) {
        console.error(e)
        result.Services.push({
            Name: "Table Storage",
            Status: "Down",
            Message: "Error",
            Health: 0.0
        })
    }

    // Status reflects table storage status (v1)
    result.Status = result.Services[0].Status
    result.Health = result.Services[0].Health

    res.send(result)
    return next()
}