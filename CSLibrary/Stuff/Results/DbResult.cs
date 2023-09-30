namespace CSLibrary.Stuff.Results
{
    public class DbResult<T> : BaseResult
    {
        public bool DbAvailable { get; set; } = true;
        public T? Entity { get; set; }
    }
}
