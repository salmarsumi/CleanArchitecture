namespace CA.Common.ResponseTypes
{
    public class JsonErrorResponse
    {
        public string ExceptionType { get; set; }
        public object Key { get; set; }
        public string Name { get; set; }
        public string DeveloperMessage { get; set; }
        public object Data { get; set; }
    }
}
