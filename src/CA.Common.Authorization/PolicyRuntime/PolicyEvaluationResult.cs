namespace CA.Common.Authorization.PolicyRuntime
{
    /// <summary>
    /// Represents the result of a policy evaluation for a specific user.
    /// </summary>
    public class PolicyEvaluationResult
    {
        public IEnumerable<string> Groups { get; set; }
        public IEnumerable<string> Permissions { get; set; }
    }
}