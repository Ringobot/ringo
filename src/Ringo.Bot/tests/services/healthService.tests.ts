import * as _healthService from "../../services/healthService";
import { expect } from 'chai';
import 'mocha';

describe('healthService.elapsedSeconds', () => {
    it('returns seconds and milliseconds', () => {
        // arrange
        let hrtime: [number, number] = [3, 414000000]
        let elapsedS = _healthService.elapsedMs(hrtime)
        expect(elapsedS).to.eq(3414)
    });
});
