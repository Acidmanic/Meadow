using Meadow.Scaffolding.Macros;

namespace Example.Macros.MySql.Macros
{
    public class InsertPersonMacro:IMacro
    {
        public string Name { get; } = "InsertPerson";
        
        public string GenerateCode(params string[] arguments)
        {
            var name = GetOrWhat(arguments, 0, "New");
            var surname = GetOrWhat(arguments, 1, "Person");
            var ageString = GetOrWhat(arguments, 2, "32");
            var jobIdString = GetOrWhat(arguments, 3, "3");

            var code =  $"INSERT INTO Persons (Name,Surname,Age,JobId) " +
                   $"VALUES ('{name}','{surname}',{ageString},{jobIdString});";

            return code;
        }


        private string GetOrWhat(string[] values, int index, string orWhat)
        {
            return values.Length > index ? values[index] : orWhat;
        }
        
    }
}