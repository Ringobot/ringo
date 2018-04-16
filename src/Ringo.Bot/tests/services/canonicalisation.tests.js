"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const canonical = require("../../services/canonicalisation");
const chai_1 = require("chai");
require("mocha");
describe('canonical.getArtistId', () => {
    it('should return a token', () => {
        let token = canonical.getArtistId('Radiohead');
        console.log(token.Id);
        chai_1.expect(token).to.not.be.null;
    });
});
