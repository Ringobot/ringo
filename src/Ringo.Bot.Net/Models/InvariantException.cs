using System;

namespace RingoBotNet.Models
{
    public class InvariantException : Exception
    {
        public InvariantException(string message) : base(message)
        {
        }
    }

    public class InvariantNullException : InvariantException
    {
        public InvariantNullException(string paramName) : base($"\"{paramName}\" must not be null")
        {
        }
    }

}
