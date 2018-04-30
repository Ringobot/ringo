import * as spotify from "../../services/spotify";
import * as spotifyAuth from "../../services/spotifyauth";
import { expect } from 'chai';
import 'mocha';
import './helper';

describe('spotify.getAuthToken', () => {
  
  it('should return a token', async () => {
    let token = await spotifyAuth.getClientAuthToken();
    expect(token).to.not.be.null;
  }).timeout(3000);

});

describe('spotify.searchArtists', function () {

  var data;

  before(async function() {
    data = await spotify.searchArtists("Radiohead");
  });
  
  it('should return a response', () => {
    expect(data).to.not.be.null;
  });

  it('should return data.artists.items', () => {
    expect(data.artists.items).to.not.be.null;
  });

});
