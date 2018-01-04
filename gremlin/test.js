"use strict";
/// <reference path="bundle.d.ts" />
Object.defineProperty(exports, "__esModule", { value: true });
const Gremlin = require("gremlin");
const client = Gremlin.createClient();
const gremlin = Gremlin.makeTemplateTag(client);
client.execute('g.V()', (err, results) => {
    if (!err) {
        console.log(results); // notice how results is *always* an array
    }
});
const name = 'Blake';
gremlin `g.V().has('name', ${name})` // template tag that returns a Promise
    .then((vertices) => {
    console.log(vertices);
})
    .catch((err) => {
    // Something went wrong
});
