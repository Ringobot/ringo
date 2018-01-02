import * as canonical from "../../services/canonicalisation";
import { expect } from 'chai';
import 'mocha';

describe('canonical.getArtistId', () => {
  it('should return a token', () => {
    let token = canonical.getArtistId('Radiohead');
    console.log(token.Id);
    expect(token).to.not.be.null;
  });
});
