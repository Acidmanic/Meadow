using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Acidmanic.Utilities.Filtering;
using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;

namespace Meadow.Extensions;

public static class MeadowConfigurationUtilityExtensions
{



    public static void ForceColumnSizeFor<TModel, TField>(
        this MeadowConfiguration configurations,
        Expression<Func<TModel, TField>> select,
        int columnSize)
    {
        var address = MemberOwnerUtilities.GetAddress(select);

        var key = address.ToLower();

        var removings = new List<string>();
        
        foreach (var item in configurations.ExternallyForcedColumnSizesByNodeAddress)
        {
            if (item.Key.ToLower() == key)
            {
                removings.Add(item.Key);
            }
        }
        
        removings.ForEach( r=> configurations.ExternallyForcedColumnSizesByNodeAddress.Remove(r));
        
        configurations.ExternallyForcedColumnSizesByNodeAddress.Add(address,columnSize);
    }
}