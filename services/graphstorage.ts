/// <reference path="../gremlin/index.d.ts" />

import _gremlin = require('gremlin');

export function createClient() {
    return _gremlin.createClient(
        443,
        process.env.GRAPH_ENDPOINT,
        {
            "session": false,
            "ssl": true,
            "user": `/dbs/${process.env.GRAPH_DATABASE_ID}/colls/${process.env.GRAPH_COLLECTION_ID}`,
            "password": process.env.GRAPH_ACCESS_KEY
        });
}

export function dropGraph(client, callback) {
    console.log('Running Drop');
    client.execute('g.V().drop()', {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }

        console.log("Result: %s\n", JSON.stringify(results));
        callback(null)
    });
}

export function addVertex1(client, callback) {
    console.log('Running Add Vertex1');
    console.log(`process.env.GRAPH_ENDPOINT = ${process.env.GRAPH_ENDPOINT}`);
    //console.log(client);

    client.execute("g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44).property('userid', 1)", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }

        console.log("Result: %s\n", JSON.stringify(results));
        callback(null)
    });
}

export function addVertex2(client, callback) {
    console.log('Running Add Vertex2');
    client.execute("g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39).property('userid', 2)", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }

        console.log("Result: %s\n", JSON.stringify(results));
        callback(null)
    });
}

export function addEdge(client, callback) {
    console.log('Running Add Edge');
    client.execute("g.V('thomas').addE('knows').to(g.V('mary'))", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }

        console.log("Result: %s\n", JSON.stringify(results));
        callback(null)
    });
}

export function countVertices(client, callback) {
    console.log('Running Count');
    client.execute("g.V().count()", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }

        console.log("Result: %s\n", JSON.stringify(results));
        callback(null)
    });
}

