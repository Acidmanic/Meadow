using Meadow.Configuration;
using Meadow.DataAccessResolving;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Extensions;
using Meadow.Test.Functional.Models;
using Meadow.Test.Functional.TestEnvironment;
using Meadow.Test.Functional.TestEnvironment.Utility;
using Xunit;
using Xunit.Abstractions;

namespace Meadow.Test.Functional.Suits;

public class SnippetsTranslationSuit
{

    private class Context
    {
        public MeadowConfiguration MeadowConfiguration { get; }
    
        public Context(Databases databases)
        {
            var setup = new MeadowEngineSetup();
        
            setup.SelectDatabase(databases);

            setup.CreateEngine();

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
    [InlineData(Databases.MySql,CommonSnippets.CreateTable)]
    [InlineData(Databases.MySql,CommonSnippets.SaveProcedure)]
    private void Should_Translate_TableScripts(Databases database, CommonSnippets snippets)
    {
        var context = new Context(database);
        
        var snippet = context.InstantiateSnippet(snippets);

        var generatedSnippet = snippet.Generate(context.MeadowConfiguration, typeof(Person));

        AssertSnippetIsGenerated(generatedSnippet);
        
        _output.WriteLine(generatedSnippet);
    }



    private void AssertSnippetIsGenerated(string generatedSnippet)
    {
        Assert.NotNull(generatedSnippet);
        
        Assert.NotEmpty(generatedSnippet);

        Assert.False(string.IsNullOrWhiteSpace(generatedSnippet));
    }
}