using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Meadow.Attributes;

namespace Meadow.Utility;

public class FullTreeMark
{
    public static bool IsMarkedFullTreeRead()
    {
        var stack = new StackTrace();

        var frames = stack.GetFrames() ?? new StackFrame[] { };

        var methods = frames.Where(f => f != null)
            .Select(f => f.GetMethod());

        foreach (var method in methods)
        {
            if (method.GetCustomAttributes<FullTreeReadAttribute>().Any())
            {
                return true;
            }

            var type = method.DeclaringType;

            if (type != null)
            {
                var ftrs = type.GetCustomAttributes<FullTreeReadAttribute>();

                foreach (var ftr in ftrs)
                {
                    if (ftr.AcceptsAsMarked(method.Name))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
}