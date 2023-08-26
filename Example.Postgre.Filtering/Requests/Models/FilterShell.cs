namespace Example.Postgre.Filtering.Requests.Models
{
    public class FilterShell
    {
        public string FilterHash { get; set; }
        
        public int ExpirationTimeStamp { get; set; }
        
        public string FilterExpression { get; set; }
    }
}

