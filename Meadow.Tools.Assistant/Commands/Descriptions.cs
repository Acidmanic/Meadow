namespace Meadow.Tools.Assistant.Commands
{
    public static class Descriptions
    {
        public const string CreateSqlScript = "Creates build-up scripts based on models found on current directory (PWD) " +
                                     "\n\t\t   and any database objects already exists in the database. The scrip created," +
                                     "\n\t\t   would be placed at the build-up scripts directory. For this command to work, " +
                                     "\n\t\t   you need to have an implementation of IMeadowConfigurationProvider in your " +
                                     "\n\t\t   project for assistant tool to find.";
        
        public const string CreatePolicies =
            "Any number of policy strings separated by ','. Policy strings can be any of values: " +
            "\n\tskip:\tskips any database object that already exists" +
            "\n\tp-skip:\tskips any procedure object that already exists" +
            "\n\tt-skip:\tskips any table object that already exists" +
            "\n\talter:\talters any database object that already exists" +
            "\n\tp-alter:\talters any procedure object that already exists" +
            "\n\tt-alter:\talters any table object that already exists" +
            "\n\tdrop:\tdrops any database object that already exists" +
            "\n\tp-drop:\tdrops any procedure object that already exists" +
            "\n\tt-drop:\tdrops any table object that already exists" +
            "\n\tskip-<name>:\tskips any database object with the given <name> that already exists" +
            "\n\talter-<name>:\talters any database object with the given <name> that already exists" +
            "\n\tdrop-<name>:\tdrops any database object with the given <name> that already exists";

        public const string CreateBlank =
            "Creates a blank sql file inside your buildup scripts directory named regarding meado conventions.";
        
    }
}