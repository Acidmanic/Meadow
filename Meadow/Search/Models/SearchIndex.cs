using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.DataTypeMapping.Attributes;

namespace Meadow.Search.Models;

public class SearchIndex<TId>
{
 
    
    [AutoValuedMember]
    [UniqueMember]
    public long Id { get; set; }
    
    
    public TId ResultId { get; set; }
    
    [ForceColumnSize(1024)]
    public string IndexCorpus { get; set; }
    
    
}