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
require("mocha");
const chai_1 = require("chai");
require("./helper");
const _artists = require("../../services/artists");
describe('artists.searchArtists', function () {
    var data;
    before(function () {
        return __awaiter(this, void 0, void 0, function* () {
            data = yield _artists.searchArtists("Radiohead");
        });
    });
    it('should return a response', () => {
        chai_1.expect(data).to.not.be.null;
    });
    it('should return image[0] URL', () => {
        chai_1.expect(data[0].images[0].url).to.not.be.undefined;
    });
    it('should return image[1] URL', () => {
        chai_1.expect(data[0].images[1].url).to.not.be.undefined;
    });
    it('should return spotify URI', () => {
        chai_1.expect(data[0].spotify.uri).to.not.be.undefined;
    });
    it('should return spotify Id', () => {
        chai_1.expect(data[0].spotify.id).to.not.be.undefined;
    });
});
describe('artists.getRelatedArtists', function () {
    var data;
    before(function () {
        return __awaiter(this, void 0, void 0, function* () {
            data = yield _artists.getRelatedArtists("3vbKDsSS70ZX9D2OcvbZmS");
        });
    });
    it('should return a response', () => {
        chai_1.expect(data).to.not.be.null;
    });
    it('should return an array', () => {
        chai_1.expect(data.length).to.be.greaterThan(0);
    });
});
