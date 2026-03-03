namespace service.Exceptions
{
    public class InvalidRequestException : RuleValidationException
    {
        public InvalidRequestException() { }
        public InvalidRequestException(string message) : base(message) { }
        public InvalidRequestException(string message, System.Exception inner) : base(message, inner) { }
    }
}