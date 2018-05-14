export async function getHealth(req, res, next) {
    // version
    var packageJson = require('../package.json')
    
    let result = {
        Status: 'Ok',
        Version: packageJson.version,
        DateTime: new Date()
    }

    res.send(result)
    return next()
}