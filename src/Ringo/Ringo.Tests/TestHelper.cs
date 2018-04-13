using Microsoft.Extensions.Configuration;
using Ringo.Common.Models;
using System;
using System.Collections.Generic;
using System.Text;

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

        public static Entity NewEntity(string type)
        {
            var properties = new Dictionary<string, string>();
            properties.Add("type", type);
            properties.Add("testProp1", "testProp1");
            properties.Add("testProp2", "testProp2");
            return new Entity(Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), properties);
        }
    }
}
