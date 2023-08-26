using System;

namespace Meadow.Extensions;

public static class GuidSearchIdExtensions
{



    public static string SearchId(this Guid guid)
    {
        return guid.ToString("N");
    }
    
}