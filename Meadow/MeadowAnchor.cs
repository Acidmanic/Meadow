using System.Reflection;

namespace Meadow;

public class TheMeadow
{

    public class AnchorType
    {
        internal AnchorType()
        {
            
        }
    }

    public static AnchorType Anchor { get; } = new AnchorType();
    
    

}

public static class MeadowAnchorAssemblyExtensions
{

    public static Assembly GetMeadowAssembly(this TheMeadow.AnchorType a)
    {
        return typeof(TheMeadow).Assembly;
    }
}