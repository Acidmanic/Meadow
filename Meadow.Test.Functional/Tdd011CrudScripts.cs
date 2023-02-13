using Acidmanic.Utilities.Reflection;
using Meadow.Configuration;
using Meadow.Contracts;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.SqlScriptsGenerators;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TDDAbstractions;

namespace Meadow.Test.Functional
{
    public class Tdd011CrudScripts : MeadowFunctionalTest
    {

        public Tdd011CrudScripts():base("MeadowScratch")
        {
            
        }

        public class TestConfigProvider : IMeadowConfigurationProvider
        {
            public MeadowConfiguration GetConfigurations()
            {
                return new MeadowConfiguration
                {
                    ConnectionString = "Server=localhost;" +
                                       "User Id=sa; " +
                                       "Password=never54aga.1n;" +
                                       $@"Database={"MeadowScratch"}; " +
                                       "MultipleActiveResultSets=true",
                    BuildupScriptDirectory = "Scripts"
                };
            }
        } 
     
        public override void Main()
        {
            var typesInvolved = TypeCheck.EnumerateEntities(typeof(Person));

            var tables = "";
            var inserts = "";
            var reads = "";
            var deletes = "";
            var updates = "";

            typesInvolved.ForEach(y =>
            {
                tables += new TableScriptGenerator(y).Generate(SqlScriptActions.Create).Text;

                inserts += new InsertProcedureGenerator(y).Generate(SqlScriptActions.Create).Text;

                reads += new ReadProcedureGenerator(y, true, true).Generate().Text;
                reads += new ReadProcedureGenerator(y, false, true).Generate().Text;
                reads += new ReadProcedureGenerator(y, true, false).Generate().Text;
                reads += new ReadProcedureGenerator(y, false, false).Generate().Text;

                deletes += new DeleteProcedureGenerator(y, false).Generate().Text;
                deletes += new DeleteProcedureGenerator(y, true).Generate().Text;

                updates += new UpdateProcedureGenerator(y).Generate().Text;
            });

            // Console.WriteLine(tables + inserts + reads + deletes + updates);
            //
            // Console.WriteLine("========================================");
            //
            // var engin = SetupClearDatabase();
            //
            // Console.WriteLine("========================================");
            //
            // var allprocedures = engin.EnumerateProcedures();
            //
            // allprocedures.ForEach(Console.WriteLine);
            //
            //
            // Console.WriteLine("========================================");
            //
            // var allTables = engin.EnumerateTables();
            //
            // allTables.ForEach(Console.WriteLine);
            //
            // Console.WriteLine("========================================");
            //
            // var models = new TypeAcquirer().EnumerateModels(".","Meadow.Test");
            //
            // models.ForEach(m => Console.WriteLine(m.Name));
            //
            // Console.WriteLine("========================================");
            //
            // var script = new AutoScriptGenerator().Generate("/home/diego/Projects/Learning/chopogh/","",
            //     new OnExistsPolicyManager().Add( o => OnExistsPolicies.Alter));
            //
            //
            // if (script.Created)
            // {
            //     var path = script.ScriptInfo.ScriptFile.FullName;
            //
            //     if (File.Exists(path))
            //     {
            //         File.Delete(path);
            //     }
            //     File.WriteAllText(path,script.ScriptInfo.Script);    
            // }
            //
            // Console.WriteLine(script.Log);
        }

    }
}