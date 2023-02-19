namespace Meadow.SQLite.ProcedureProcessing
{
    public interface IParsable
    {

        bool Parse(string sql);
    }
}