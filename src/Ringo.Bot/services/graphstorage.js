"use strict";
/// <reference path="../gremlin/index.d.ts" />
Object.defineProperty(exports, "__esModule", { value: true });
const _gremlin = require("gremlin");
if (!process.env.GRAPH_ENDPOINT || !process.env.GRAPH_DATABASE_ID || !process.env.GRAPH_COLLECTION_ID || !process.env.GRAPH_ACCESS_KEY) {
    throw new Error('Expecting Env vars GRAPH_ENDPOINT, GRAPH_DATABASE_ID, GRAPH_COLLECTION_ID, GRAPH_ACCESS_KEY');
}
let client = _gremlin.createClient(443, process.env.GRAPH_ENDPOINT, {
    "session": false,
    "ssl": true,
    "user": `/dbs/${process.env.GRAPH_DATABASE_ID}/colls/${process.env.GRAPH_COLLECTION_ID}`,
    "password": process.env.GRAPH_ACCESS_KEY
});
function getVertexByName(name) {
    var query = `g.V().hasLabel('${name}')`;
    console.log('graphstorage.getVertexByName', 'vertex.Name', name);
    console.debug('graphstorage.addVertex', 'query', query);
    return new Promise((resolve, reject) => {
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
exports.getVertexByName = getVertexByName;
;
function vertexEdgeVertices(vertexId, edgeLabel, verticesType) {
    // g.V().has('id', 'default-user').out('likes').has('type', 'artist')
    var query = `g.V().has('id', vertexId).out(edgeLabel).has('type', verticesType)`;
    var bindings = { vertexId, edgeLabel, verticesType };
    return new Promise((resolve, reject) => {
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
exports.vertexEdgeVertices = vertexEdgeVertices;
function getVertexById(id) {
    // g.V().hasId('123')
    var query = `g.V().hasId('${id}')`;
    console.log('graphstorage.getVertexByName', 'vertex.Id', id);
    console.debug('graphstorage.addVertex', 'query', query);
    return new Promise((resolve, reject) => {
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
exports.getVertexById = getVertexById;
function addVertex(vertex) {
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
    return new Promise((resolve, reject) => {
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
exports.addVertex = addVertex;
;
function addEdgeLike(edge) {
    var query = `g.V('${edge.FromVertex}').addE('${edge.Relationship}').to(g.V('${edge.ToVertex}'))`;
    console.log('graphstorage.addEdgeLike', 'FromVertex', edge.FromVertex);
    console.log('graphstorage.addEdgeLike', 'Relationship', edge.Relationship);
    console.log('graphstorage.addEdgeLike', 'ToVertex', edge.ToVertex);
    console.debug('graphstorage.addEdgeLike', 'query', query);
    return new Promise((resolve, reject) => {
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
exports.addEdgeLike = addEdgeLike;
;