using System;

namespace Meadow.Extensions;

public static class EnumExtensions
{
    public static bool Is<T>(this T value, T flag)
        where T : struct
    {
       
        if (!value.GetType().IsEnum || !flag.GetType().IsEnum)
        {
            return false;
        }

        return ((Convert.ToUInt64(value) & Convert.ToUInt64(flag)) > 0);
    }
}