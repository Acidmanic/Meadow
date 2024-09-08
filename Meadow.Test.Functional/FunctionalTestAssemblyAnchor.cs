using System.Reflection;

namespace Meadow.Test.Functional;

public static class FunctionalTestAssemblyAnchor
{
    public static Assembly Assembly => typeof(FunctionalTestAssemblyAnchor).Assembly;
}