namespace service.Exceptions
{
    public class DuplicateResourceException : RuleValidationException
    {
        public DuplicateResourceException() { }
        public DuplicateResourceException(string message) : base(message) { }
        public DuplicateResourceException(string message, System.Exception inner) : base(message, inner) { }
    }
}