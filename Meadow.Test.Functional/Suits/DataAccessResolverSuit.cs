using System;
using System.Collections.Generic;
using Meadow.Configuration;
using Meadow.DataAccessResolving;
using Meadow.MySql;
using Meadow.Postgre;
using Meadow.SQLite;
using Meadow.SQLite.Extensions;
using Meadow.SqlServer;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class DataAccessResolverSuit
{

    private class Context
    {
        private MeadowEngine _engine ;

        public MeadowConfiguration Configuration => new MeadowConfiguration();

        public Context()
        {
            _engine = new MeadowEngine(Configuration);
        }
        public void UseDataAccess(string name)
        {
            
            if (name == "sqlite") _engine.UseSqLite();
            else if (name == "mysql") _engine.UseMySql();
            else if (name == "sqlserver") _engine.UseSqlServer();
            else if (name == "postgre") _engine.UsePostgre();
            
        }
    }

    private readonly ITestOutputHelper _outputHelper;

    public DataAccessResolverSuit(ITestOutputHelper outputHelper)
    {
        _outputHelper = outputHelper;
    }

    [Theory]
    [InlineData("sqlite")]
    [InlineData("mysql")]
    [InlineData("sqlserver")]
    [InlineData("postgre")]
    public void Should_Resolve_DbTypeNameMapper(string dataAccessName)
    {
        var context = new Context();
        
        context.UseDataAccess(dataAccessName);

        var resolver = new DataAccessServiceResolver(context.Configuration);

        var mapper = resolver.DbTypeNameMapper;

        Assert.NotNull(mapper);
        
        _outputHelper.WriteLine("DA: {0},Found DbTypeNameMapper: {1}",dataAccessName, mapper.GetType());

        var translator = resolver.SqlExpressionTranslator;
        
        Assert.NotNull(translator);
        
        _outputHelper.WriteLine("DA: {0},Found SqlExpressionTranslator: {1}",dataAccessName, translator.GetType());
    }
    
}