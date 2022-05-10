namespace CA.Common.Exceptions
{
    public class ConcurrencyException : Exception
    {
        public ConcurrencyException() : base() { }

        public ConcurrencyException(string message) : base(message) { }

        public ConcurrencyException(string message, Exception ex): base(message, ex) { }
    }
}
