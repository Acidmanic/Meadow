using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meadow.BuildupScripts;
using Meadow.Reflection.FetchPlug;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.Contracts;
using Meadow.Scaffolding.OnExistsPolicy;
using Meadow.Scaffolding.SqlScriptsGenerators;

namespace Meadow.Scaffolding
{
    public class AutoScriptGenerator
    {
        public ScriptInfo Generate(string directory, string nameSpace, OnExistsPolicyManager policyManager)
        {
            var configurationProvider
                = new TypeAcquirer().AcquireAny<IMeadowConfigurationProvider>(directory)
                    .SingleOrDefault();

            if (configurationProvider == null)
            {
                // meeeeh

                return null;
            }

            var configurations = configurationProvider.GetConfigurations();

            var engine = new MeadowEngine(configurations);

            var existingTables = engine.EnumerateTables();
            var existingProcedures = engine.EnumerateProcedures();

            bool AlreadyExists(string name, DbObjectTypes type)
            {
                if (type == DbObjectTypes.Tables)
                {
                    return existingTables.Contains(name);
                }

                if (type == DbObjectTypes.StoredProcedures)
                {
                    return existingProcedures.Contains(name);
                }

                return false;
            }

            var models = new TypeAcquirer().EnumerateModels(directory, nameSpace);

            var sqlGenerators = new List<SqlGeneratorBase>();

            foreach (var model in models)
            {
                sqlGenerators.Add(new TableScriptGenerator(model));
                sqlGenerators.Add(new InsertProcedureGenerator(model));

                sqlGenerators.Add(new ReadProcedureGenerator(model, false, false));
                sqlGenerators.Add(new ReadProcedureGenerator(model, false, true));
                sqlGenerators.Add(new ReadProcedureGenerator(model, true, true));
                sqlGenerators.Add(new ReadProcedureGenerator(model, true, false));


                sqlGenerators.Add(new DeleteProcedureGenerator(model, true));
                sqlGenerators.Add(new DeleteProcedureGenerator(model, false));

                sqlGenerators.Add(new UpdateProcedureGenerator(model));
            }

            var objectsCreated = 0;
            var tablesCreated = 0;
            var proceduresCreated = 0;
            var script = "";

            void Create(SqlGeneratorBase gen, SqlScriptActions action)
            {
                objectsCreated++;
                if (gen.ObjectType == DbObjectTypes.Tables)
                {
                    tablesCreated++;
                }

                if (gen.ObjectType == DbObjectTypes.StoredProcedures)
                {
                    proceduresCreated++;
                }

                script += gen.Generate(action).Text;
            }

            foreach (var generator in sqlGenerators)
            {
                if (AlreadyExists(generator.SqlObjectName, generator.ObjectType))
                {
                    var policyAction = policyManager.OnExists(generator.SqlObjectName, generator.ObjectType);

                    if (policyAction == OnExistsPolicies.Alter)
                    {
                        Create(generator, SqlScriptActions.Alter);
                    }
                    else if (policyAction == OnExistsPolicies.DropAndReCreate)
                    {
                        Create(generator, SqlScriptActions.DropCreate);
                    }
                }
                else
                {
                    Create(generator, SqlScriptActions.Create);
                }
            }

            if (objectsCreated == 0)
            {
                return null;
            }


            var name = $"add-{tablesCreated}-tables-and-{proceduresCreated}-procedures";

            var order = "xxxx";

            var fileName = order + "-" + name + ".sql";

            var filePath = Path.Combine(new DirectoryInfo(configurations.BuildupScriptDirectory).FullName, fileName);

            return new ScriptInfo
            {
                Name = name,
                Order = order,
                ScriptFile = new FileInfo(filePath),
                Script = script
            };
        }
    }
}