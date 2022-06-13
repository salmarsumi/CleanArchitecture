namespace CA.Common.Authorization.PolicyRuntime
{
    public class Permission
    {
        public string Name { get; set; }
        public IEnumerable<string> Roles { get; set; }

        internal bool Evaluate(IEnumerable<string> roles)
        {
            if(roles is null)
            {
                throw new ArgumentNullException(nameof(roles));
            }

            if(Roles.Any(r => roles.Contains(r)))
            {
                return true;
            }

            return false;
        }
    }
}
