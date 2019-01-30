using Microsoft.Extensions.Configuration;

namespace RingoBotNet.Helpers
{
    internal static class ConfigHelper
    {
        public const string CosmosDBEndpoint = "CosmosDBEndpoint";
        public const string CosmosDBPrimaryKey = "CosmosDBPrimaryKey";
        public const string CosmosDBDatabaseName = "CosmosDBDatabaseName";
        public const string CosmosDBChannelUserCollectionName = "CosmosDBChannelUserCollectionName";
        public const string CosmosDBUserStateCollectionName = "CosmosDBUserStateCollectionName";

        public static void CheckConfig(IConfiguration configuration)
        {
            if (string.IsNullOrEmpty(configuration[CosmosDBEndpoint]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBEndpoint}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBPrimaryKey]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBPrimaryKey}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBDatabaseName]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBDatabaseName}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBChannelUserCollectionName]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBChannelUserCollectionName}\" is required.");
            if (string.IsNullOrEmpty(configuration[CosmosDBUserStateCollectionName]))
                throw new ConfigurationMissingException($"App Setting \"{CosmosDBUserStateCollectionName}\" is required.");
        }
    }
}
