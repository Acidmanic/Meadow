using Meadow.Configuration;
using Meadow.SQLite;
using Meadow.Test.Functional.Models;
using Meadow.Utility;
using Xunit;

namespace Meadow.Test.Functional.Suits;

public class ProcessedTypeCollectiveIdSuit
{




    [Fact]
    public void Should_Get_CorrectProfile()
    {

        var processedType = EntityTypeUtilities.Process<Address>(new MeadowConfiguration(),new SqLiteTypeNameMapper());

        
        
    }
}