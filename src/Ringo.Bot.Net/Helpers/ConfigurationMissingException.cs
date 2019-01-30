using System;

namespace RingoBotNet.Helpers
{
    public class ConfigurationMissingException : Exception
    {
        public ConfigurationMissingException(string message) : base(message) { }
    }
}
