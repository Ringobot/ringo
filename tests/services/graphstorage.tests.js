"use strict";
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
const _graphDb = require("../../services/graphstorage");
const chai_1 = require("chai");
require("mocha");
const helper = require("./helper");
const dbId = 'ringobot'; //TODO: Change to ringobot_test
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
    before(function () {
        return __awaiter(this, void 0, void 0, function* () {
            results = yield _graphDb.addVertex(client, vertex);
            console.log(results);
        });
    });
    it('has label', function () {
        chai_1.expect(results[0].label).equals(name);
    });
    it('has Country property', function () {
        chai_1.expect(results[0].properties.Country[0].value).equals("UK");
    });
    it('has Genre property', function () {
        chai_1.expect(results[0].properties.Genre[0].value).equals("Alternative");
    });
    after(function () {
        // TODO: delete Vertex
    });
});
