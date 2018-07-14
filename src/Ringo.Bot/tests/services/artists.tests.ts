import 'mocha';
import { expect } from 'chai';
import './helper';

import _artists = require('../../services/artists');
import artist = require('../../models/artist');

describe('artists.searchArtists', function () {
    var data: artist.Artist[];

    before(async function () {
        this.timeout(10000);
        data = await _artists.searchArtists("Radiohead");
    });

    it('should return a response', () => {
        expect(data).to.not.be.null;
    });

    it('should return image[0] URL', () => {
        expect(data[0].images[0].url).to.not.be.undefined
    });

    it('should return image[1] URL', () => {
        expect(data[0].images[1].url).to.not.be.undefined
    });

    it('should return spotify URI', () => {
        expect(data[0].spotify.uri).to.not.be.undefined
    });

    it('should return spotify Id', () => {
        expect(data[0].spotify.id).to.not.be.undefined
    });

})

describe('artists.getRelatedArtists', function () {
    var data: artist.Artist[];

    before(async function () {
        this.timeout(10000);
        data = await _artists.getRelatedArtists("3vbKDsSS70ZX9D2OcvbZmS");
    });

    it('should return a response', () => {
        expect(data).to.not.be.null;
    });
 
    it('should return an array', () => {
        expect(data.length).to.be.greaterThan(0);
    });
    
})