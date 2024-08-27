using System;
using Meadow.Configuration;
using Meadow.DataAccessResolving;
using Meadow.Extensions;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Extensions;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

[Collection("SEQUENTIAL_DATABASE_TESTS")]
public class SnippetsTranslationSuit
{

    private class Context
    {
        public MeadowConfiguration MeadowConfiguration { get; }
    
        public Context(Databases databases)
        {
            var setup = new MeadowEngineSetup();
            
            setup.SelectDatabase(databases);
            
            setup.CreateEngine(c =>
            {
                c.AddFilter<Address>(b => b.Where(a => a.IsDeleted).IsEqualTo(false));
                c.AddFilter<Person>(b => b.Where(p => p.IsDeleted).IsEqualTo(false));    
            });

            MeadowConfiguration = setup.Configuration;
        }

        public ISnippet InstantiateSnippet(CommonSnippets snippets) 
            => new DataAccessServiceResolver(MeadowConfiguration).InstantiateSnippet(snippets)!;
    }

    private readonly ITestOutputHelper _output;


    public SnippetsTranslationSuit(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [InlineData(Databases.SqLite,CommonSnippets.CreateTable)]
    [InlineData(Databases.SqLite,CommonSnippets.SaveProcedure)]
    [InlineData(Databases.SqLite,CommonSnippets.FullTreeView)]
    [InlineData(Databases.SqLite,CommonSnippets.DataBound)]
    [InlineData(Databases.SqLite,CommonSnippets.EventStreamScript)]
    [InlineData(Databases.SqLite,CommonSnippets.FindPaged)]
    [InlineData(Databases.SqLite,CommonSnippets.InsertProcedure)]
    [InlineData(Databases.SqLite,CommonSnippets.ReadProcedure,IdAwarenessBehavior.UseIdAware)]
    [InlineData(Databases.MySql,CommonSnippets.CreateTable)]
    [InlineData(Databases.MySql,CommonSnippets.SaveProcedure)]
    [InlineData(Databases.MySql,CommonSnippets.FullTreeView)]
    public void Should_Translate_TableScripts(Databases database, CommonSnippets snippets,IdAwarenessBehavior behavior = IdAwarenessBehavior.UseNone)
    {
        var context = new Context(database);
        
        var snippet = context.InstantiateSnippet(snippets);

        var generatedSnippet = snippet.Generate(context.MeadowConfiguration, GetTestEntityType(snippets),
            b =>
            {
                b.RepetitionHandling(RepetitionHandling.Alter);
                if (behavior.Is(IdAwarenessBehavior.UseAll)) b.BehaviorUseAll();
                if (behavior.Is(IdAwarenessBehavior.UseById)) b.BehaviorUseById();
            });

        AssertSnippetIsGenerated(generatedSnippet);
        
        _output.WriteLine(generatedSnippet);
    }


    private Type GetTestEntityType(CommonSnippets snippets)
    {
        if (snippets == CommonSnippets.EventStreamScript)
        {
            return typeof(BigEvent);
        }

        return typeof(Person);
    }


    private void AssertSnippetIsGenerated(string generatedSnippet)
    {
        Assert.NotNull(generatedSnippet);
        
        Assert.NotEmpty(generatedSnippet);

        Assert.False(string.IsNullOrWhiteSpace(generatedSnippet));
    }
}