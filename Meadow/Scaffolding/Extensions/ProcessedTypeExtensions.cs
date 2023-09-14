using System.Collections.Generic;
using Meadow.Scaffolding.Models;

namespace Meadow.Scaffolding.Extensions;

public static class ProcessedTypeExtensions
{
    public static List<Parameter> GetInsertParameters(this ProcessedType processedType)
    {
        return (processedType.HasId && processedType.IdField.IsAutoValued)
            ? processedType.NoneIdParameters
            : processedType.Parameters;
    }
}