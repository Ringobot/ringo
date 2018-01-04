import * as _graphDb from "../../services/graphstorage";
import { expect } from 'chai';
import 'mocha';
import './helper';

describe('addVertex', () => {

    it('does not crash', (done) => {
        let client = _graphDb.createClient();
        _graphDb.dropGraph(client, function () {
            _graphDb.addVertex1(client, function () {
                console.log('callback');
                // expect(response).to.not.be.null;
                done();
            });
        });
    }).timeout(5000);

});
