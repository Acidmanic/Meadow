using System.Collections.Generic;

namespace Meadow.Requests.Common.Models
{
    public class ChunkRequest
    {
        public string? SearchId { get; set; }
            
        public long Offset { get; set; }
            
        public long Size { get; set; }
    }
    
    public class ChunkResponse<TModel>
    {
        public string? SearchId { get; set; }
            
        public long Offset { get; set; }
            
        public long Size { get; set; }
        
        public List<TModel> Items { get; set; }
        
        public long TotalCount { get; set; }
    }
}