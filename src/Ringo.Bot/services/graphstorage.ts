/// <reference path="../gremlin/index.d.ts" />

import _gremlin = require('gremlin');

export function createClient(dbId?: string, collectionId?: string) {
    return _gremlin.createClient(
        443,
        process.env.GRAPH_ENDPOINT,
        {
            "session": false,
            "ssl": true,
            "user": `/dbs/${dbId || process.env.GRAPH_DATABASE_ID}/colls/${collectionId || process.env.GRAPH_COLLECTION_ID}`,
            "password": process.env.GRAPH_ACCESS_KEY
        });
}

/*
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
*/

interface Vertex {
    Id: string,
    Name: string,
    Properties: VertexProperty[]
}

interface VertexProperty {
    Key: string,
    Value: string
}

interface Edge {
    FromVertex: string,
    Relationship: string,
    ToVertex: string
}

export function getVertexByName(client, name: string) {
    var query = `g.V().hasLabel('${name}')`;

    console.log('graphstorage.getVertexByName', 'vertex.Name', name);
    console.debug('graphstorage.addVertex', 'query', query);

    return new Promise<any>((resolve, reject) => {
        client.execute(query, function (err, results) {
            if (err) {
                reject(err);
                return;
            }

            resolve(results);
            return;
        });
    });
};

export function getVertexById(client, id:string){
    // g.V().hasId('123')
    var query = `g.V().hasId('${id}')`;

    console.log('graphstorage.getVertexByName', 'vertex.Id', id);
    console.debug('graphstorage.addVertex', 'query', query);

    return new Promise<any>((resolve, reject) => {
        client.execute(query, function (err, results) {
            if (err) {
                reject(err);
                return;
            }

            resolve(results);
            return;
        });
    });
}

export function addVertex(client, vertex: Vertex) {
    var query = "g.addV(T.Id, _Id)";
    var bindings = { _Id: vertex.Id };

    for (var i = 0; i < vertex.Properties.length; i++) {
        query += `.property(_Property_${vertex.Properties[i].Key}, _Property_${vertex.Properties[i].Key}_Value)`;
        bindings[`_Property_${vertex.Properties[i].Key}`] = vertex.Properties[i].Key;
        bindings[`_Property_${vertex.Properties[i].Key}_Value`] = vertex.Properties[i].Value;
    }

    console.log('graphstorage.addVertex', 'vertex.Id', vertex.Id);
    console.debug('graphstorage.addVertex', 'query', query);
    console.debug('graphstorage.addVertex', 'bindings', bindings);

    return new Promise<any>((resolve, reject) => {
        client.execute(query, bindings, function (err, results) {
            if (err) {
                reject(err);
                return;
            }

            resolve(results);
            return;
        });
    });
};

export function addEdgeLike(client, edge: Edge) {
    var query = `g.V('${edge.FromVertex}').addE('${edge.Relationship}').to(g.V('${edge.ToVertex}'))`;

    console.log('graphstorage.addEdgeLike', 'FromVertex', edge.FromVertex);
    console.log('graphstorage.addEdgeLike', 'Relationship', edge.Relationship);
    console.log('graphstorage.addEdgeLike', 'ToVertex', edge.ToVertex);
    console.debug('graphstorage.addEdgeLike', 'query', query);

    return new Promise<any>((resolve, reject) => {
        client.execute(query, function (err, results) {
            if (err) {
                reject(err);
                return;
            }

            resolve(results);
            return;
        });
    });
};