using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Scaffolding.Macros;

public class Coffee:IMacro
{
    public string Name { get; } = "Coffee";
    public string GenerateCode(params string[] arguments)
    {
        return @"
-- ----------------------------------------------------------------------------------
--                                  Coffee Time!
-- ----------------------------------------------------------------------------------
--                                      )))
--                                      (((
--                                    +-----+
--                                    |     |]
--                                    `-----'  
--                                  ___________
--                                  `---------' 
-- ----------------------------------------------------------------------------------
--             (James Christopher Goodwin - https://ascii.co.uk/art/coffee)
-- ----------------------------------------------------------------------------------
";
    }

    public List<Assembly> LoadedAssemblies { get; set; } = new List<Assembly>();
}