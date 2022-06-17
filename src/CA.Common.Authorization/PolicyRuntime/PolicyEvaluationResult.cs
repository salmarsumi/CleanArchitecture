namespace CA.Common.Authorization.PolicyRuntime
{
    public class PolicyEvaluationResult
    {
        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}