namespace service.Exceptions
{
    public class ResourceNotFoundException : RuleValidationException
    {
        public ResourceNotFoundException() { }
        public ResourceNotFoundException(string message) : base(message) { }
        public ResourceNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    }
}