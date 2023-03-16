using System;
using CoreCommandLine;
using CoreCommandLine.Attributes;
using CoreCommandLine.CommonCommandBases;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.SQLite;
using Meadow.SqlServer;

namespace Meadow.Tools.Assistant.Commands.Arguments
{
    [CommandName("database","db")]
    public class DatabaseType:ParameterCommandBase
    {

        public static string Key => nameof(DatabaseType);
        
        protected override void RetrieveData(Context context, string parameterStringValue)
        {
            
            switch (parameterStringValue)
            {
                case "mssql-server":
                    context.Set<Action<MeadowEngine>>(Key,e => e.UseSqlServer());
                    break;
                case "my-sql":
                    context.Set<Action<MeadowEngine>>(Key,e => e.UseMySql());
                    break;
                case "postgre":
                    context.Set<Action<MeadowEngine>>(Key,e => e.UsePostgre());
                    break;
                case "sqlite":
                    context.Set<Action<MeadowEngine>>(Key,e => e.UseSQLite());
                    break;
            }
        }

        public override string Description => "This will determine the database type which meadow has to use to " +
                                              "connect to project's database. the correct values are: mssql-server," +
                                              "my-sql, postgre, sqlite. (not case sensitive)";
    }

    public static class ContextDatabaseTypeExtensions
    {

        public static Context SetDatabaseType(this Context context, MeadowEngine engine)
        {

            context.Get<Action<MeadowEngine>>(DatabaseType.Key, m => { })(engine);

            return context;
        }
    }
}