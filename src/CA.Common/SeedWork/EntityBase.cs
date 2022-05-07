namespace CA.Common.SeedWork
{
    public abstract class EntityBase<T> : IAuditable, IConcurrency
    {
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
