/// <reference path="../gremlin/index.d.ts" />

import _gremlin = require('gremlin');

if (!process.env.GRAPH_ENDPOINT || !process.env.GRAPH_DATABASE_ID || !process.env.GRAPH_COLLECTION_ID || !process.env.GRAPH_ACCESS_KEY) {
    throw new Error('Expecting Env vars GRAPH_ENDPOINT, GRAPH_DATABASE_ID, GRAPH_COLLECTION_ID, GRAPH_ACCESS_KEY')
}

let client = _gremlin.createClient(
    443,
    process.env.GRAPH_ENDPOINT,
    {
        "session": false,
        "ssl": true,
        "user": `/dbs/${process.env.GRAPH_DATABASE_ID}/colls/${process.env.GRAPH_COLLECTION_ID}`,
        "password": process.env.GRAPH_ACCESS_KEY
    }
);

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

export function execute(query, bindings) {
    console.debug('graphstorage.execute: query, bindings', query, bindings);
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
}

export function getVertexByName(name: string) {
    var query = 'g.V().hasLabel(name)'
    var bindings = {name}
    return execute(query, bindings);
};

export function vertexEdgeVertices(vertexId, edgeLabel, verticesType) {
    // g.V().has('id', 'default-user').out('likes').has('type', 'artist')
    var query = `g.V().has('id', vertexId).out(edgeLabel).has('type', verticesType)`;
    var bindings = { vertexId, edgeLabel, verticesType };
    return execute(query, bindings);
}

export function getVertexById(id: string) {
    // g.V().hasId('123')
    var query = 'g.V().hasId(id)';
    let bindings = {id}

    return execute(query, bindings);
}

export function addVertex(vertex: Vertex) {
    var query = "g.addV(T.Id, _Id)";
    var bindings = { _Id: vertex.Id };

    for (var i = 0; i < vertex.Properties.length; i++) {
        query += `.property(_Property_${vertex.Properties[i].Key}, _Property_${vertex.Properties[i].Key}_Value)`;
        bindings[`_Property_${vertex.Properties[i].Key}`] = vertex.Properties[i].Key;
        bindings[`_Property_${vertex.Properties[i].Key}_Value`] = vertex.Properties[i].Value;
    }

    console.debug('graphstorage.addVertex', 'vertex.Id', vertex.Id);
    console.debug('graphstorage.addVertex', 'query', query);
    console.debug('graphstorage.addVertex', 'bindings', bindings);
    
    return execute(query, bindings);
};

export function addEdgeLike(edge: Edge) {
    var query = 'g.V(FromVertex).addE(Relationship).to(g.V(ToVertex))'

    console.debug('graphstorage.addEdgeLike', 'FromVertex', edge.FromVertex);
    console.debug('graphstorage.addEdgeLike', 'Relationship', edge.Relationship);
    console.debug('graphstorage.addEdgeLike', 'ToVertex', edge.ToVertex);
    console.debug('graphstorage.addEdgeLike', 'query', query);
    
    return execute(query, edge);
};
