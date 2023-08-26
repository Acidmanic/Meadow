namespace Example.Postgre.Filtering.Requests.Models
{
    public class ChunkShell
    {
        public string FilterHash { get; set; }

        public long Offset { get; set; }

        public long Size { get; set; }
    }
}