namespace Meadow.SQLite.ProcedureProcessing
{
    public abstract class DbObjectCreation:ISqlable,IParsable
    {
        public abstract string ToSql();

        private class ObjectCreation : DbObjectCreation
        {
            private string _creation;
            
            public ObjectCreation(string creation)
            {
                _creation = creation;
            }
            public override string ToSql()
            {
                return _creation.ToUpper();
            }

            public override bool Parse(string sql)
            {
                if (sql == null)
                {
                    return false;
                }
                sql = sql.Trim();
                if (string.IsNullOrEmpty(sql))
                {
                    return false;
                }

                _creation = sql;
                return true;
            }
        }
        
        public static readonly DbObjectCreation Create = new ObjectCreation("Create");
        public static readonly DbObjectCreation Drop = new ObjectCreation("Alter");
        public abstract bool Parse(string sql);
    }
}