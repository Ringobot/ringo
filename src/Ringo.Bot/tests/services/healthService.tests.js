"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const _healthService = require("../../services/healthService");
const chai_1 = require("chai");
require("mocha");
describe('healthService.elapsedSeconds', () => {
    it('returns seconds and milliseconds', () => {
        // arrange
        let hrtime = [3, 414000000];
        let elapsedS = _healthService.elapsedMs(hrtime);
        chai_1.expect(elapsedS).to.eq(3414);
    });
});
