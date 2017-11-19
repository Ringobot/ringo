import * as spotify from "../../services/spotify";
import { expect } from 'chai';
import 'mocha';

describe('spotify.getAuthToken', () => {
  it('should return a token', async () => {
    let token = await spotify.getAuthToken();
    console.log(token);
    expect(token).to.not.be.null;
  }).timeout(5000);
});

describe('spotify.searchArtists', () => {
  it('should return a response', async () => {
    let artists = await spotify.searchArtists("Radiohead");
    console.log(artists);
    expect(artists).to.not.be.null;
  }).timeout(5000);
});
