namespace Meadow.SQLite.ProcedureProcessing
{
    public class DbObjectCreation : ISqlable, IParsable
    {
        // public abstract string ToSql();
        //
        // private class ObjectCreation : DbObjectCreation
        // {
        private string _creation;

        public DbObjectCreation(string creation)
        {
            _creation = creation;
        }

        public string ToSql()
        {
            return _creation.ToUpper();
        }

        public virtual bool Parse(string sql)
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

            foreach (var creation in StandardDbObjectCreations)
            {
                sql = sql.ToLower();

                if (creation._creation.ToLower() == sql)
                {
                    _creation = creation._creation;
                    break;
                }
            }

            _creation = sql;

            return true;
        }
        // }

        private class ReadonlyDbObjectCreation : DbObjectCreation
        {
            public ReadonlyDbObjectCreation(string creation) : base(creation)
            {
            }

            public override bool Parse(string sql)
            {
                return false;
            }
        }


        public static readonly DbObjectCreation Create = new ReadonlyDbObjectCreation("Create");
        public static readonly DbObjectCreation Drop = new ReadonlyDbObjectCreation("Drop");
        public static readonly DbObjectCreation DropIfExists = new ReadonlyDbObjectCreation("Drop If Exists");
        public static readonly DbObjectCreation Alter = new ReadonlyDbObjectCreation("Alter");
        public static readonly DbObjectCreation CreateIfNotExists = new ReadonlyDbObjectCreation("Create If Not Exists");
        public static readonly DbObjectCreation CreateOrAlter = new ReadonlyDbObjectCreation("Create Or Alter");

        private static readonly DbObjectCreation[] StandardDbObjectCreations =
        {
            Create,
            CreateIfNotExists,
            CreateOrAlter,
            Drop,
            DropIfExists,
            Alter
        };


        public static bool operator ==(DbObjectCreation d1, DbObjectCreation d2)
        {
            if (d1 == null && d2 == null)
            {
                return true;
            }

            if (d1 == null || d2 == null)
            {
                return false;
            }

            return d1._creation?.ToLower() == d2._creation?.ToLower();
        }

        public static bool operator !=(DbObjectCreation d1, DbObjectCreation d2)
        {
            return !(d1 == d2);
        }

        public override bool Equals(object obj)
        {
            if (obj is DbObjectCreation other)
            {
                return _creation?.ToLower() == other._creation?.ToLower();
            }

            return false;
        }

        protected bool Equals(DbObjectCreation other)
        {
            return _creation?.ToLower() == other._creation?.ToLower();
        }

        public override int GetHashCode()
        {
            if (string.IsNullOrWhiteSpace(_creation))
            {
                return 0;
            }

            return _creation.GetHashCode();
        }
    }
}