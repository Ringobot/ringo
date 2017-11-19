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
const httpj = require("../../services/httpjson");
const chai_1 = require("chai");
require("mocha");
describe('httpjson.request', () => {
    it('returns response from GET /posts', () => __awaiter(this, void 0, void 0, function* () {
        let response = yield httpj.request("get", "https://jsonplaceholder.typicode.com/posts");
        chai_1.expect(response).to.not.be.null;
    }));
});
describe('httpjson.get', () => {
    it('returns response from GET /posts', () => __awaiter(this, void 0, void 0, function* () {
        let response = yield httpj.get("https://jsonplaceholder.typicode.com/posts");
        chai_1.expect(response).to.not.be.null;
    }));
});
describe('httpjson.post', () => {
    it('returns response from POST /posts', () => __awaiter(this, void 0, void 0, function* () {
        const data = { userId: 1, id: 1, title: "title", body: "Body" };
        let response = yield httpj.post("https://jsonplaceholder.typicode.com/posts", data);
        chai_1.expect(response).to.not.be.null;
    }));
});
