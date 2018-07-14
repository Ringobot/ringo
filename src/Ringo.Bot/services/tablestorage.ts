import _azure = require('azure-storage');
import _common = require('azure-common');

export interface TableEntity {
    PartitionKey: string,
    RowKey: string
}

export function createQuery(): _azure.TableQuery {
    return new _azure.TableQuery();
}

// Inserts entity. Invokes callback(error) when done.
// table = table name
export function insert(table: string, entity: TableEntity, insertOrReplace: boolean = false): Promise<TableEntity> {
    var tableService = getTableService();

    return new Promise<any>((resolve, reject) => {
        var entry = mapToEntry(entity);
        let callback = function (error, data) {
            if (error) {
                reject(error);
                return;
            }
            console.log(`tablestorage ${insertOrReplace ? 'insertOrReplace' : 'insert'} into ${table}`);
            resolve(mapToEntity(data));
            return;
        };

        insertOrReplace
            ? tableService.insertOrReplaceEntity(table, entry, callback)
            : tableService.insertEntity(table, entry, callback);
    })
}

export function update(table: string, entity: TableEntity): Promise<TableEntity> {
    var tableService = getTableService();

    return new Promise<any>((resolve, reject) => {
        tableService.replaceEntity(table, entity, function (error, data) {
            if (error) {
                reject(error);
                return;
            }

            resolve(mapToEntity(data));
        });
    }
    )
}

// Deletes entity. Invokes callback(error) when done.
export function deleteEntity(table: string, entity: TableEntity, callback) {
    var tableService = getTableService();
    tableService.deleteEntity(table, { PartitionKey: entity.PartitionKey, RowKey: entity.RowKey }, function (error) {
        if (error) {
            callback(error);
            return;
        }
        console.log("tablestorage deleted from ", table);
        callback(null);
    });
};

// Returns entities from query. Invokes callback(error, entities) when done.
export function get(table, query: _azure.TableQuery) {
    var tableService = getTableService();

    return new Promise<any>((resolve, reject) => {
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

export function createTableIfNotExists(table: string): Promise<any> {
    var tableService = getTableService();

    return new Promise<any>((resolve, reject) => {
        tableService.createTableIfNotExists(table, function (error) {
            if (error) {
                reject(error);
                return;
            }
            resolve();
        });
    })
};

// Maps an Array of Azure Table Storage entries to an Array of simple JS objects
var mapToEntities = function (entries): TableEntity[] {
    var entities = [];
    for (var i = 0; i < entries.length; i++) {
        entities[i] = mapToEntity(entries[i]);
    }
    return entities;
};

// Maps an Azure Table Storage entry to a simple JS object.
var mapToEntity = function (entry): TableEntity {
    var entity = { PartitionKey: null, RowKey: null };
    //entity._entry = entry;
    for (var key in entry) {
        entity[key] = entry[key]["_"];
    }
    return entity;
}

// Maps a simple JS object to an Azure Table Storage Entity.
// See also: https://github.com/DanielLarsenNZ/Hacking-on-Azure/wiki/Azure-Storage-SDK-for-Node.js-debacle
var mapToEntry = function (entity: TableEntity) {
    // edmType is a util library provided by azure-common.
    var edmType = _common.edmType;

    var entry = new Object();
    for (var key in entity) {
        entry[key] = { "_": entity[key], "$": edmType.propertyType(entity[key]) };
    }
    return entry;
}

var getTableService = function () {
    //TODO: Reuse Table Service?
    var tableService = _azure.createTableService();
    return tableService;
};
