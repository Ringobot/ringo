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
            return new Entity(Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"), type);
        }
    }
}
