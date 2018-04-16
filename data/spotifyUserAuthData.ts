import _table = require('../services/tablestorage');

const UserAuthTable = "UserSpotifyAuth";

let rowQuery = (userHash) => _table.createQuery().where('PartitionKey eq ? and RowKey eq ?', partitionKey(userHash), userHash)

function partitionKey(userHash: string): string {
    return userHash.substr(0, 5);
}

export interface UserAuthEntity extends _table.TableEntity {
    userHash: string,
    state?: string,
    code?: string,
    expiry?: Date,
    error?: string,
    authUrl?: string,
    accessToken?: string,
    tokenType?: string,
    scope?: string,
    refreshToken?: string
}

export async function get(userHash: string): Promise<UserAuthEntity> {
    try {
        let entities: UserAuthEntity[] = await _table.get(UserAuthTable, rowQuery(userHash));
        return entities.length === 1 ? entities[0] : null;
    } catch (e) {
        throw e
    }
}

export async function update(entity: UserAuthEntity) {
    try {
        await _table.update(UserAuthTable, entity);
    } catch (e) {
        throw e
    }
}

export function createUserAuthEntity(userHash:string, state:string): UserAuthEntity{
    return {
        PartitionKey: partitionKey(userHash),
        RowKey: userHash,
        state: state,
        userHash: userHash
    };
}

export async function insertOrReplace(entity:UserAuthEntity): Promise<_table.TableEntity>{
    try {
        return await _table.insert(UserAuthTable, entity, true);

    } catch (e) {
        throw e
    }

}
