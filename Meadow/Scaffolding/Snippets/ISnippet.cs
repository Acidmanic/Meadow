using Meadow.Configuration;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Snippets;

public interface ISnippet
{

    void Initialize(MeadowConfiguration configuration, ProcessedType processedType);
    
    
    string Template { get; }
}