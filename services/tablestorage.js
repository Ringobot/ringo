"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const _azure = require("azure-storage");
const _common = require("azure-common");
function createQuery() {
    return new _azure.TableQuery();
}
exports.createQuery = createQuery;
// Inserts entity. Invokes callback(error) when done.
// table = table name
function insert(table, entity, insertOrReplace = false) {
    var tableService = getTableService();
    return new Promise((resolve, reject) => {
        createTableIfNotExists(tableService, table, function (error) {
            if (error) {
                reject(error);
                return;
            }
            var entry = mapToEntry(entity);
            let callback = function (error, data) {
                if (error) {
                    reject(error);
                    return;
                }
                console.log(`tablestorage ${insertOrReplace ? 'insertOrReplace' : 'insert'} into ${table}`);
                resolve(data);
                return;
            };
            insertOrReplace
                ? tableService.insertOrReplaceEntity(table, entry, callback)
                : tableService.insertEntity(table, entry, callback);
        });
    });
}
exports.insert = insert;
;
function update(table, entity, callback) {
    var tableService = getTableService();
    tableService.replaceEntity(table, entity, function (error) {
        if (error) {
            callback(error);
            return;
        }
        callback(null);
    });
}
exports.update = update;
;
// Deletes entity. Invokes callback(error) when done.
function deleteEntity(table, entity, callback) {
    var tableService = getTableService();
    tableService.deleteEntity(table, { PartitionKey: entity.PartitionKey, RowKey: entity.RowKey }, function (error) {
        if (error) {
            callback(error);
            return;
        }
        console.log("tablestorage deleted from ", table);
        callback(null);
    });
}
exports.deleteEntity = deleteEntity;
;
// Returns entities from query. Invokes callback(error, entities) when done.
function get(table, query) {
    var tableService = getTableService();
    return new Promise((resolve, reject) => {
        tableService.queryEntities(table, query, null, function (error, result, response) {
            if (error) {
                reject(error);
                return;
            }
            console.log("tablestorage got entries ", result.entries.length);
            // map storage entries to simple JS objects
            var entities = mapToEntities(result.entries);
            resolve(entities);
            return;
        });
    });
}
exports.get = get;
// Maps an Array of Azure Table Storage entries to an Array of simple JS objects
var mapToEntities = function (entries) {
    var entities = [];
    for (var i = 0; i < entries.length; i++) {
        entities[i] = mapToEntity(entries[i]);
    }
    return entities;
};
// Maps an Azure Table Storage entry to a simple JS object.
var mapToEntity = function (entry) {
    var entity = { PartitionKey: null, RowKey: null };
    //entity._entry = entry;
    for (var key in entry) {
        entity[key] = entry[key]["_"];
    }
    return entity;
};
// Maps a simple JS object to an Azure Table Storage Entity.
// See also: https://github.com/DanielLarsenNZ/Hacking-on-Azure/wiki/Azure-Storage-SDK-for-Node.js-debacle
var mapToEntry = function (entity) {
    // edmType is a util library provided by azure-common.
    var edmType = _common.edmType;
    var entry = new Object();
    for (var key in entity) {
        entry[key] = { "_": entity[key], "$": edmType.propertyType(entity[key]) };
    }
    return entry;
};
var createTableIfNotExists = function (tableService, table, callback) {
    tableService.createTableIfNotExists(table, function (error) {
        if (error) {
            callback(error);
            return;
        }
        callback(null);
    });
};
var getTableService = function () {
    var tableService = _azure.createTableService();
    return tableService;
};
