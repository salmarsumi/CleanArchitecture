namespace CA.Common.SeedWork
{
    public interface IConcurrency
    {
        int RowVersion { get; set; }

        void IncrementVersion();
    }
}
