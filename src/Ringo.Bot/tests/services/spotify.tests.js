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
        chai_1.expect(token).to.not.be.null;
    })).timeout(3000);
});
describe('spotify.searchArtists', function () {
    var data;
    before(function () {
        return __awaiter(this, void 0, void 0, function* () {
            data = yield spotify.searchArtists("Radiohead");
        });
    });
    it('should return a response', () => {
        chai_1.expect(data).to.not.be.null;
    });
    it('should return data.artists.items', () => {
        chai_1.expect(data.artists.items).to.not.be.null;
    });
});
