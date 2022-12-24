using System;

namespace BroWar.Debugging.Console
{
    internal class ArgumentExtractionException : Exception
    {
        public ArgumentExtractionException()
        { }

        public ArgumentExtractionException(string message) : base(message)
        { }
    }
}