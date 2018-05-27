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
const _graph = require("../../services/graphstorage");
const chai_1 = require("chai");
require("mocha");
describe('graphstorage', () => {
    it('vertexEdgeVertices returns not null', () => __awaiter(this, void 0, void 0, function* () {
        const userId = 'defaultUser';
        let data = yield _graph.vertexEdgeVertices(userId, 'likes', 'artist');
        chai_1.expect(data).to.not.be.null;
    }));
    it('execute returns not null', () => __awaiter(this, void 0, void 0, function* () {
        const userId = 'defaultUser';
        let data = yield _graph.execute('g.V().limit(limit)', { limit: 1 });
        chai_1.expect(data).to.not.be.null;
    }));
});
