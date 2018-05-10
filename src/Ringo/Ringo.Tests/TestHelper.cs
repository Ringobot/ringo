using Microsoft.Extensions.Configuration;
using Ringo.Common.Models;
using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ringo.Tests
{
    public class TestHelper
    {
        public static IConfigurationRoot GetIConfigurationRoot()
        {
            return new ConfigurationBuilder()
                .AddJsonFile("appsettings.test.json")
                .AddEnvironmentVariables()
                .Build();
        }

        public static Entity NewEntity(string name, string type)
        {
            var guid = Guid.NewGuid().ToString("N");
            var properties = new JObject();
            properties.Add("type", type);

            return new Entity($"{name}:{guid}", name, properties);
        }

        public static Entity NewEntity(string id, string name, string type)
        {
            var properties = new JObject();
            properties.Add("type", type);
            properties.Add("spotifyId", "3YcBF2ttyueytpXtEzn1Za");
            properties.Add("spotifyUri", "spotify:artist:3YcBF2ttyueytpXtEzn1Za");
            properties.Add("imageUrl", "https://i.scdn.co/image/f723e8e0109e5ef3d18f83fcbae0c81cc49bd577");

            return new Entity(id, name, properties);
        }
    }
}
