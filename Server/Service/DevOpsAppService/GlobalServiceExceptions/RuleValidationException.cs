namespace service.Exceptions
{
    public class RuleValidationException : System.Exception
    {
        public RuleValidationException() { }
        public RuleValidationException(string message) : base(message) { }
        public RuleValidationException(string message, System.Exception inner) : base(message, inner) { }
    }
}