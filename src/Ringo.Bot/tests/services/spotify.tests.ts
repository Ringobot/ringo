import * as spotify from "../../services/spotify";
import * as spotifyAuth from "../../services/spotifyauth";
import { expect } from 'chai';
import 'mocha';
import './helper';

describe('spotify.getAuthToken', () => {
  
  it('should return a token', async () => {
    let token = await spotifyAuth.getClientAuthToken();
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