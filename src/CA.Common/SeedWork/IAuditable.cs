namespace CA.Common.SeedWork
{
    /// <summary>
    /// Entity audit properties.
    /// </summary>
    public interface IAuditable
    {
        /// <summary>
        /// The name of the user created the entity.
        /// </summary>
        string CreatedBy { get; set; }

        /// <summary>
        /// The timestamp when the entity was created.
        /// </summary>
        DateTime Created { get; set; }

        /// <summary>
        /// The name of the user last modified the entity.
        /// </summary>
        string LastModifiedBy { get; set; }

        /// <summary>
        /// The timestamp when the entity was modified.
        /// </summary>
        DateTime LastModified { get; set; }
    }
}
