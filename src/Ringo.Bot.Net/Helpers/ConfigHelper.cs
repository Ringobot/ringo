using Microsoft.Extensions.Configuration;

namespace RingoBotNet.Helpers
{
    internal static class ConfigHelper
    {
        public const string CosmosDBEndpoint = "CosmosDBEndpoint";
        public const string CosmosDBPrimaryKey = "CosmosDBPrimaryKey";
        public const string CosmosDBDatabaseName = "CosmosDBDatabaseName";
        public const string CosmosDBUserCollectionName = "CosmosDBUserCollectionName";
        public const string StorageConnectionString = "StorageConnectionString";
        public const string StorageStateContainer = "StorageStateContainer";
        public const string BotServiceEndpointAppId = "BotServiceEndpointAppId";
        public const string BotServiceEndpointAppPassword = "BotServiceEndpointAppPassword";
        public const string StateTableName = "StateTableName";
        public const string CosmosDBStationCollectionName = "CosmosDBStationCollectionName";

        public static void CheckConfig(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration[CosmosDBEndpoint]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBEndpoint}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBPrimaryKey]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBPrimaryKey}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBDatabaseName]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBDatabaseName}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBUserCollectionName]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBUserCollectionName}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBStationCollectionName]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBStationCollectionName}\" is required.");
            if (string.IsNullOrEmpty(configuration[StorageConnectionString]))
                throw new ConfigurationMissingException($"App Setting \"{StorageConnectionString}\" is required.");
            if (string.IsNullOrEmpty(configuration[StorageStateContainer]))
                throw new ConfigurationMissingException($"App Setting \"{StorageStateContainer}\" is required.");
            if (string.IsNullOrEmpty(configuration[StateTableName]))
                throw new ConfigurationMissingException($"App Setting \"{StateTableName}\" is required.");
        }
    }
}
