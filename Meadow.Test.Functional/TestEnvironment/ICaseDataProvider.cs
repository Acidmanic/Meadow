
using System.Collections.Generic;

namespace Meadow.Test.Functional.TestEnvironment;

public interface ICaseDataProvider
{


    void Initialize();

    void PostSeeding();
    
    List<List<object>> SeedSet { get; }
}
