using System;

namespace OctaNotes.Scripts.Play.Model
{
    public class ChartParseException : Exception
    {
        private const string template = "ChartParser : {0} {1} ";

        protected ChartParseException() {}
        protected ChartParseException(string message) : base(String.Format(template, message)) { }

        protected ChartParseException(string message, System.Exception inner) : base(message, inner) { }
    }
    
    public class InvalidPrefixException : ChartParseException
    {
        private const string message = "不正なPREFIXです。";
        public InvalidPrefixException() { }

        public InvalidPrefixException(string message) : base(message) { }

        public InvalidPrefixException(string message, System.Exception inner) : base(message, inner) { }
    }
    
}
