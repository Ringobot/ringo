"use strict";
/// <reference path="../gremlin/index.d.ts" />
Object.defineProperty(exports, "__esModule", { value: true });
const _gremlin = require("gremlin");
function createClient(dbId, collectionId) {
    return _gremlin.createClient(443, process.env.GRAPH_ENDPOINT, {
        "session": false,
        "ssl": true,
        "user": `/dbs/${dbId || process.env.GRAPH_DATABASE_ID}/colls/${collectionId || process.env.GRAPH_COLLECTION_ID}`,
        "password": process.env.GRAPH_ACCESS_KEY
    });
}
exports.createClient = createClient;
function addVertex(client, vertex) {
    var query = "g.addV(_Vertex_Name)";
    var bindings = { _Vertex_Name: vertex.Name };
    for (var i = 0; i < vertex.Properties.length; i++) {
        query += `.property(_Property_${vertex.Properties[i].Key}, _Property_${vertex.Properties[i].Key}_Value)`;
        bindings[`_Property_${vertex.Properties[i].Key}`] = vertex.Properties[i].Key;
        bindings[`_Property_${vertex.Properties[i].Key}_Value`] = vertex.Properties[i].Value;
    }
    console.log('graphstorage.addVertex', 'vertex.Name', vertex.Name);
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
function addVertex1(client, callback) {
    console.log('Running Add Vertex1');
    console.log(`process.env.GRAPH_ENDPOINT = ${process.env.GRAPH_ENDPOINT}`);
    //console.log(client);
    client.execute("g.addV('person').property('id', 'thomas').property('firstName', 'Thomas').property('age', 44).property('userid', 1)", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }
        console.log("Result: %s\n", JSON.stringify(results));
        callback(null);
    });
}
exports.addVertex1 = addVertex1;
function addVertex2(client, callback) {
    console.log('Running Add Vertex2');
    client.execute("g.addV('person').property('id', 'mary').property('firstName', 'Mary').property('lastName', 'Andersen').property('age', 39).property('userid', 2)", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }
        console.log("Result: %s\n", JSON.stringify(results));
        callback(null);
    });
}
exports.addVertex2 = addVertex2;
function addEdge(client, callback) {
    console.log('Running Add Edge');
    client.execute("g.V('thomas').addE('knows').to(g.V('mary'))", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }
        console.log("Result: %s\n", JSON.stringify(results));
        callback(null);
    });
}
exports.addEdge = addEdge;
function countVertices(client, callback) {
    console.log('Running Count');
    client.execute("g.V().count()", {}, (err, results) => {
        if (err) {
            return callback(console.error(err));
        }
        console.log("Result: %s\n", JSON.stringify(results));
        callback(null);
    });
}
exports.countVertices = countVertices;
