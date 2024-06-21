namespace CA.Common.Authorization.PolicyRuntime
{
    /// <summary>
    /// Represents a single system permission with all the 
    /// groups assigned that specific permission.Given a 
    /// collection of groups the permission instance can 
    /// evaluate whether these groups have been granted 
    /// the specified permission.
    /// </summary>
    public class Permission
    {
        public string Name { get; set; }
        public IEnumerable<string> Groups { get; set; }

        /// <summary>
        /// Evaluate whether a collection of groups are assigned the current permission.
        /// </summary>
        /// <param name="groups">The collection of groups that needs to be evaluated.</param>
        /// <returns>The <see cref="bool"/> value indicating whether any of the groups are assigned the current permission.</returns>
        /// <exception cref="ArgumentNullException" />
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
