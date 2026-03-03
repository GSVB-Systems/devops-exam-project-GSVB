namespace service.Exceptions
{
    public class RangeValidationException : RuleValidationException
    {
        public RangeValidationException() { }
        public RangeValidationException(string message) : base(message) { }
        public RangeValidationException(string message, System.Exception inner) : base(message, inner) { }
    }
}