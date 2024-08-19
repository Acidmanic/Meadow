using System;
using System.Linq;
using System.Runtime.InteropServices;
using Acidmanic.Utilities.Filtering.Utilities;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.Suits.DataProviders;
using Meadow.Test.Functional.TestEnvironment;
using Xunit;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class CrudSuit
{
    private const Databases Databases = TestEnvironment.Databases.SqLite;

    private readonly Func<Person, string> _personIdentifier = p => $"{p.Name}:{p.Id}";


    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void ShouldBeAble_ToRead_ById(bool fullTree)
    {
        
    }
    
}