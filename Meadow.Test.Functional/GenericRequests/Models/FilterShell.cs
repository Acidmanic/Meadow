namespace Meadow.Test.Functional.GenericRequests.Models
{
    public class FilterShell
    {
        public long ExpirationTimeStamp { get; set; }

        public string FilterExpression { get; set; }

        public string SearchExpression { get; set; }
        
        public string OrderExpression { get; set; }

        public string SearchId { get; set; }
    }
}