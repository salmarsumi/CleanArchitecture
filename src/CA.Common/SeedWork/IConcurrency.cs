namespace CA.Common.SeedWork
{
    /// <summary>
    /// Entity concurrency conflict checks.
    /// </summary>
    public interface IConcurrency
    {
        /// <summary>
        /// The version of the entity.
        /// This property will be used for concurency conflict checks.
        /// </summary>
        int RowVersion { get; set; }

        /// <summary>
        /// Increment the current entity version.
        /// </summary>
        void IncrementVersion();
    }
}
