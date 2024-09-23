using System;
using System.Linq;
using Meadow.Contracts;
using Meadow.Extensions;
using Meadow.Requests.GenericEventStreamRequests.Models;
using Meadow.Scaffolding.Attributes;
using Meadow.Scaffolding.Extensions;
using Meadow.Scaffolding.Macros.BuiltIn.Snippets;
using Meadow.Scaffolding.Models;
using Meadow.Scaffolding.Snippets;
using Meadow.Scaffolding.Snippets.Builtin;

namespace Meadow.MySql.Snippets;

[CommonSnippet(CommonSnippets.EventStreamScript)]
public class EventStreamSnippet : ISnippet
{
    public ISnippetToolbox Toolbox { get; set; } = SnippetToolbox.Null;
    
    private ProcessedType ProcessedType => Toolbox.ProcessedType;
    private NameConvention NameConvention => Toolbox.ProcessedType.NameConvention;

    private RepetitionHandling RepetitionHandling => Toolbox.Configurations.RepetitionHandling;

    public string InsertProcedure => Toolbox.TranslateEventStreamsPhraseInsertProcedure();
    
    public string ReadAllStreamsProcedure(string body) => Toolbox.Procedure(Toolbox.Configurations.RepetitionHandling,
        Toolbox.ProcessedType.NameConvention.ReadAllStreams, body);
    
    public ISnippet SelectReadAllStreamsSelect => new SelectAllSnippet() 
    
    public string Template => @"
-- ---------------------------------------------------------------------------------------------------------------------
{InsertProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
{ReadAllStreamsProcedure}
{SelectReadAllStreamsSelect}
{/ReadAllStreamsProcedure}
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
-- ---------------------------------------------------------------------------------------------------------------------
".Trim();
}