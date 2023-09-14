using System.Reflection;

namespace Meadow;

public class Meadow
{

    public class AnchorType
    {

    }

    public static AnchorType Anchor { get; } = new AnchorType();
    
    

}

public static class MeadowAnchorAssemblyExtensions
{

    public static Assembly GetMeadowAssembly(this Meadow.AnchorType a)
    {
        return typeof(Meadow).Assembly;
    }
}