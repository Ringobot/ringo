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
const spotify = require("../../services/spotify");
const spotifyAuth = require("../../services/spotifyauth");
const chai_1 = require("chai");
require("mocha");
require("./helper");
describe('spotify.getAuthToken', () => {
    it('should return a token', () => __awaiter(this, void 0, void 0, function* () {
        let token = yield spotifyAuth.getClientAuthToken();
        console.log(token);
        chai_1.expect(token).to.not.be.null;
    })).timeout(5000);
});
describe('spotify.searchArtists', () => {
    it('should return a response', () => __awaiter(this, void 0, void 0, function* () {
        let artists = yield spotify.searchArtists("Radiohead");
        console.log(artists);
        chai_1.expect(artists).to.not.be.null;
    })).timeout(5000);
});
