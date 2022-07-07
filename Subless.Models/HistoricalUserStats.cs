namespace Subless.Models
{
    public class HistoricalStats<T>
    {
        public T thisMonth { get; set; }
        public T LastMonth { get; set; }
    }
}
