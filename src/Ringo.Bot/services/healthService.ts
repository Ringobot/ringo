import _table = require('./tablestorage')
import errs = require('restify-errors');
import _graph = require('./graphstorage')

/**
 * Restify handler for Health API
 * @param req 
 * @param res 
 * @param next 
 */
export async function getHealth(req, res, next) {
    // version number from package.json
    var packageJson = require('../package.json')

    // default result
    let result = {
        Status: 'Up',
        Version: packageJson.version,
        DateTime: new Date(),
        Services: []
    }

    try {
        let results = await Promise.all([getTableStorageHealth(), getGraphDbHealth()])
        result.Services = results;

        // business logic for determining system status. TODO: Test
        result.Status = (
            results.find(s=>s.Status === "Down") || 
            results.find(s=>s.Status === "Trouble") || 
            results.find(s=>s.Status === "Up")
        ).Status

        res.send(result)
        return next()
        
    } catch (e) {
        console.error(e)
        next(new errs.InternalServerError())
    }
}

/**
 * Helper function to convert hi-res time into milliseconds
 * @param hrtime tuple of seconds and nanoseconds
 */
export function elapsedMs(hrtime:[number, number]): number{
    return (hrtime[0] * 1000) + (hrtime[1] / 1000000)
}

/**
 * Standard result for individual service health
 */
interface ServiceHealth {
    Name: string,
    Status: string,
    Elapsed?: number,
    Message?: string
}

/**
 * Determines the health of a Table Storage account and returns a ServiceHealth result
 */
async function getTableStorageHealth() : Promise<ServiceHealth> {
    // table storage
    try {
        // use a high-res timer
        let start = process.hrtime()
        // get top 1 from a table TODO: Health table
        await _table.get('UserSpotifyAuth', _table.createQuery().top(1))
        let elapsed = elapsedMs(process.hrtime(start))

        // Business logic for deciding if Table Storage is Up, Trouble or Down
        let health = 
            // <100ms = Up
            elapsed < 100 ? 'Up' : 
            // < 5 seconds = Trouble
            elapsed < 3000 ? 'Trouble' : 
            // > 5 seconds = Down
            'Down'

        return {
            Name: "Table Storage",
            Status: health,
            Elapsed: elapsed
        }

    } catch (e) {
        // error means Down
        console.error(e)
        return {
            Name: "Table Storage",
            Status: "Down",
            Message: "Error"
        }
    }
    
}

/**
 * Determines the health of Graph Db and returns a ServiceHealth result
 */
async function getGraphDbHealth() : Promise<ServiceHealth> {
    try {
        // use a high-res timer
        let start = process.hrtime()

        await _graph.execute('g.V().limit(1)', undefined)

        let elapsed = elapsedMs(process.hrtime(start))

        // Business logic for deciding if Up, Trouble or Down
        let health = 
            // <100ms = Up
            elapsed < 100 ? 'Up' : 
            // < 5 seconds = Trouble
            elapsed < 3000 ? 'Trouble' : 
            // > 5 seconds = Down
            'Down'

        return {
            Name: "Graph Db",
            Status: health,
            Elapsed: elapsed
        }

    } catch (e) {
        // error means Down
        console.error(e)
        return {
            Name: "Graph Db",
            Status: "Down",
            Message: "Error"
        }
    }
    
}