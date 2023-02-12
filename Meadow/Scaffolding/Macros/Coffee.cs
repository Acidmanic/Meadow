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
}