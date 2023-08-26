namespace Example.Postgre.Filtering.Requests.Models
{
    public class ChunkShell
    {
        public string FilterHash { get; set; }

        public int Offset { get; set; }

        public int Size { get; set; }
    }
}