using System;
using System.Threading.Channels;
using Meadow.Configuration;
using Meadow.Reflection;
using Meadow.Reflection.FetchPlug;
using Meadow.Scaffolding.Contracts;
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
                tables += new TableScriptGenerator(y).Generate(false).Text;

                inserts += new InsertProcedureGenerator(y).Generate(false).Text;

                reads += new ReadProcedureGenerator(y, true, true).Generate().Text;
                reads += new ReadProcedureGenerator(y, false, true).Generate().Text;
                reads += new ReadProcedureGenerator(y, true, false).Generate().Text;
                reads += new ReadProcedureGenerator(y, false, false).Generate().Text;

                deletes += new DeleteProcedureGenerator(y, false).Generate().Text;
                deletes += new DeleteProcedureGenerator(y, true).Generate().Text;

                updates += new UpdateProcedureGenerator(y).Generate().Text;
            });

            Console.WriteLine(tables + inserts + reads + deletes + updates);
            
            Console.WriteLine("========================================");

            var engin = SetupClearDatabase();
            
            Console.WriteLine("========================================");

            var allprocedures = engin.EnumerateProcedures();
            
            allprocedures.ForEach(Console.WriteLine);

            
            Console.WriteLine("========================================");

            var allTables = engin.EnumerateTables();
            
            allTables.ForEach(Console.WriteLine);
            
            Console.WriteLine("========================================");
            
            var models = new TypeAcquirer().EnumerateModels(".");

            
        }

    }
}