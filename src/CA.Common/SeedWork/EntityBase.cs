namespace CA.Common.SeedWork
{
    /// <summary>
    /// Base class for all Entities in the system.
    /// </summary>
    /// <typeparam name="T">The type of the Id required for the current entity.</typeparam>
    public abstract class EntityBase<T> : IAuditable, IConcurrency
    {
        /// <summary>
        /// Identity property.
        /// </summary>
        public T Id { get; set; }

        public string CreatedBy { get; set; }
        public DateTime Created { get; set; }
        public string LastModifiedBy { get; set; }
        public DateTime LastModified { get; set; }

        
        public int RowVersion { get; set; }

        public void IncrementVersion()
        {
            RowVersion++;
        }
    }
}
