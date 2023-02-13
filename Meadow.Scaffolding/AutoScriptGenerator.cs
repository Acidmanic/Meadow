using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Meadow.BuildupScripts;
using Meadow.Configuration;
using Meadow.Reflection;
using Meadow.Scaffolding.CodeGenerators;
using Meadow.Scaffolding.OnExistsPolicy;
using Meadow.Scaffolding.SqlScriptsGenerators;

namespace Meadow.Scaffolding
{
    public class AutoScriptGenerator
    {
        public class ScriptGeneratingResult
        {
            public ScriptInfo ScriptInfo { get; set; }

            public bool Created { get; set; }

            public string Log { get; set; }
        }

        public ScriptGeneratingResult Generate(string directory, List<Type> availableTypes, string nameSpace,
            OnExistsPolicyManager policyManager)
        {
            var configurationProvider
                = new TypeAcquirer()
                    .AcquireAny<IMeadowConfigurationProvider>(availableTypes)
                    .FirstOrDefault();

            if (configurationProvider == null)
            {
                return new ScriptGeneratingResult
                {
                    Created = false,
                    Log = "Could not find any constructable ConfigurationProvider in your project."
                };
            }

            var configurations = configurationProvider.GetConfigurations();

            var engine = new MeadowEngine(configurations);

            var existingTables = engine.EnumerateTables();
            var existingProcedures = engine.EnumerateProcedures();

            bool AlreadyExists(string objectName, DbObjectTypes type)
            {
                if (type == DbObjectTypes.Tables)
                {
                    return existingTables.Contains(objectName);
                }

                if (type == DbObjectTypes.StoredProcedures)
                {
                    return existingProcedures.Contains(objectName);
                }

                return false;
            }

            var models = new TypeAcquirer().EnumerateModels(availableTypes, nameSpace);

            var codeGenerators = new List<ICodeGenerator>();

            foreach (var model in models)
            {
                codeGenerators.Add(new CommentSectionTitleGenerator(model.Name + " Table(s)"));

                codeGenerators.Add(new TableScriptGenerator(model));
                
                codeGenerators.Add(new SqlSplitterGenerator());

                codeGenerators.Add(new CommentSectionTitleGenerator(model.Name + " Procedures"));

                codeGenerators.Add(new InsertProcedureGenerator(model));

                codeGenerators.Add(new SqlSingleLineGenerator());
                
                codeGenerators.Add(new ReadProcedureGenerator(model, false, false));
                
                codeGenerators.Add(new SqlSingleLineGenerator());
                
                codeGenerators.Add(new ReadProcedureGenerator(model, false, true));
                
                codeGenerators.Add(new SqlSingleLineGenerator());
                
                codeGenerators.Add(new ReadProcedureGenerator(model, true, true));
                
                codeGenerators.Add(new SqlSingleLineGenerator());
                
                codeGenerators.Add(new ReadProcedureGenerator(model, true, false));
                
                codeGenerators.Add(new SqlSingleLineGenerator());
                
                codeGenerators.Add(new DeleteProcedureGenerator(model, true));
                
                codeGenerators.Add(new SqlSingleLineGenerator());
                
                codeGenerators.Add(new DeleteProcedureGenerator(model, false));

                codeGenerators.Add(new SqlSingleLineGenerator());
                
                codeGenerators.Add(new UpdateProcedureGenerator(model));
                
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

            foreach (var generator in codeGenerators)
            {
                if (generator is SqlGeneratorBase sqlGenerator)
                {
                    if (AlreadyExists(sqlGenerator.SqlObjectName, sqlGenerator.ObjectType))
                    {
                        var policyAction = policyManager.OnExists(sqlGenerator.SqlObjectName, sqlGenerator.ObjectType);

                        if (policyAction == OnExistsPolicies.Alter)
                        {
                            Create(sqlGenerator, SqlScriptActions.Alter);
                        }
                        else if (policyAction == OnExistsPolicies.DropAndReCreate)
                        {
                            Create(sqlGenerator, SqlScriptActions.DropCreate);
                        }
                    }
                    else
                    {
                        Create(sqlGenerator, SqlScriptActions.Create);
                    }
                }
                else
                {
                    script += generator.Generate().Text;
                }
            }

            if (objectsCreated == 0)
            {
                return new ScriptGeneratingResult
                {
                    Created = false,
                    Log = "No Update script has been created."
                };
            }


            var name = $"add-{tablesCreated}-tables-and-{proceduresCreated}-procedures";

            var order = "xxxx";

            var fileName = order + "-" + name + ".sql";

            var scriptsDirectory = configurations.BuildupScriptDirectory;

            directory = Path.GetFullPath(directory);

            if (!Path.IsPathRooted(scriptsDirectory))
            {
                scriptsDirectory = Path.Join(directory, scriptsDirectory);
            }

            var filePath = Path.Combine(scriptsDirectory, fileName);

            return new ScriptGeneratingResult
            {
                Created = true,
                Log =
                    $"Created {objectsCreated} Database Objects:\n\t{tablesCreated} Tables:\n\t{proceduresCreated} Procedures.\n" +
                    $"Script name: {name}",
                ScriptInfo = new ScriptInfo
                {
                    Name = name,
                    Order = order,
                    ScriptFile = new FileInfo(filePath),
                    Script = script
                }
            };
        }
    }
}