namespace CA.Common.Authorization.PolicyRuntime
{
    public class Permission
    {
        public string Name { get; set; }
        public IEnumerable<string> Groups { get; set; }

        internal bool Evaluate(IEnumerable<string> groups)
        {
            if(groups is null)
            {
                throw new ArgumentNullException(nameof(groups));
            }

            if(Groups.Any(r => groups.Contains(r)))
            {
                return true;
            }

            return false;
        }
    }
}
