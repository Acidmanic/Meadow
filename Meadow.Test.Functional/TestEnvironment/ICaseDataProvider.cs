
using System.Collections.Generic;

namespace Meadow.Test.Functional.TestEnvironment;

public interface ICaseDataProvider
{


    void Initialize();
    
    List<List<object>> SeedSet { get; }
}
