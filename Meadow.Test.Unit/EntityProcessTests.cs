using Acidmanic.Utilities.Extensions;
using Acidmanic.Utilities.Reflection.Attributes;
using Meadow.Attributes;
using Meadow.Configuration;
using Meadow.DataAccessResolving;
using Meadow.MySql;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Utility;

namespace Meadow.Test.Unit;

public class EntityProcessTests
{

    
    [Fact]
    private void Should_Provide_Correct_EntryInfo()
    {
        var configuration = MeadowConfiguration.Null;
        MeadowEngine.UseDataAccess(new CoreProvider<MySqlDataAccessCore>());
        var resolver = new DataAccessServiceResolver(configuration);

        var processedType = EntityTypeUtilities.Process<ObjectEntry<long,Guid>>(configuration, resolver.DbTypeNameMapper);

    }
}