import * as _graphDb from "../../services/graphstorage";
import { expect } from 'chai';
import 'mocha';
import helper = require('./helper');

const dbId = 'ringobot';    //TODO: Change to ringobot_test
let client = _graphDb.createClient(dbId);

/*
describe('addVertex1', () => {

    it('does not crash', (done) => {

        _graphDb.dropGraph(client, function () {
            _graphDb.addVertex1(client, function () {
                console.log('callback');
                // expect(response).to.not.be.null;
                done();
            });
        });

    }).timeout(5000);

});
*/

describe('addVertex() results', function () {
    this.timeout(10000);
    let name = helper.newGuid();

    let vertex = {
        Id: name,
        Name: name,
        Properties: [
            { Key: "Country", Value: "UK" },
            { Key: "Genre", Value: "Alternative" }
        ]
    };

    var results;

    before(async function () {
        results = await _graphDb.addVertex(client, vertex);
        console.log(results);
    });

    it('has label', function () {
        expect(results[0].label).equals(name);
    });

    it('has Country property', function () {
        expect(results[0].properties.Country[0].value).equals("UK");
    });

    it('has Genre property', function () {
        expect(results[0].properties.Genre[0].value).equals("Alternative");
    });

    after(function () {
        // TODO: delete Vertex
    });
});
