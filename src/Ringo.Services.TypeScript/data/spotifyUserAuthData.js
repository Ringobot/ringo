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
const _table = require("../services/tablestorage");
const UserAuthTable = "UserSpotifyAuth";
let rowQuery = (userHash) => _table.createQuery().where('PartitionKey eq ? and RowKey eq ?', partitionKey(userHash), userHash);
function partitionKey(userHash) {
    return userHash.substr(0, 5);
}
function get(userHash) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            let entities = yield _table.get(UserAuthTable, rowQuery(userHash));
            return entities.length === 1 ? entities[0] : null;
        }
        catch (e) {
            throw e;
        }
    });
}
exports.get = get;
function update(entity) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            yield _table.update(UserAuthTable, entity);
        }
        catch (e) {
            throw e;
        }
    });
}
exports.update = update;
function createUserAuthEntity(userHash, state) {
    return {
        PartitionKey: partitionKey(userHash),
        RowKey: userHash,
        state: state,
        userHash: userHash
    };
}
exports.createUserAuthEntity = createUserAuthEntity;
function insertOrReplace(entity) {
    return __awaiter(this, void 0, void 0, function* () {
        try {
            return yield _table.insert(UserAuthTable, entity, true);
        }
        catch (e) {
            throw e;
        }
    });
}
exports.insertOrReplace = insertOrReplace;
