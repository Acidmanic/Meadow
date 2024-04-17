namespace Meadow.Scaffolding.Translators.Contracts;

public interface ISyntaxTranslator
{
    string QuotesColumnName(string name);

    string QuoteTableName(string name);

    string QuoteProcedureName(string name);
    
    string TableAliasQuot =>  "'";
}