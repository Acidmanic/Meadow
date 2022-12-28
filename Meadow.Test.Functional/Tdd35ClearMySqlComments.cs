using System;
using Meadow.Extensions;
using Meadow.MySql.Comments;
using Meadow.Test.Functional.TDDAbstractions;
using Microsoft.Extensions.Logging.LightWeight;

namespace Meadow.Test.Functional
{
    public class Tdd35ClearMySqlComments:MeadowFunctionalTest
    {
        
        public override void Main()
        {

            var text = "#==\n            create table Tags(\n                PropertyId bigint,\n                ProductClassId bigint\n            );\n#==============================";


            var clear = text.ClearMySqlComments().Trim();

            Console.WriteLine(clear);
        }

    }
}