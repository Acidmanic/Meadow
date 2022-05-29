namespace Meadow.Tools.Assistant.Commands
{
    public static class Descriptions
    {
        public const string Create = "Creates build-up scripts based on models found on current directory (PWD) " +
                                     "and any database objects already exists in the database. The scrip created," +
                                     "would be placed at the build-up scripts directory. For this command to work, " +
                                     "you need to have an implementation of IMeadowConfigurationProvider in your " +
                                     "project for assistant tool to find.";

        public const string CreatePolicies =
            "Any number of policy strings separated by ','. Policy strings can be any of values: " +
            "\nskip:\tskips any database object that already exists" +
            "\np-skip:\tskips any procedure object that already exists" +
            "\nt-skip:\tskips any table object that already exists" +
            "\nalter:\talters any database object that already exists" +
            "\np-alter:\talters any procedure object that already exists" +
            "\nt-alter:\talters any table object that already exists" +
            "\ndrop:\tdrops any database object that already exists" +
            "\np-drop:\tdrops any procedure object that already exists" +
            "\nt-drop:\tdrops any table object that already exists" +
            "\nskip-<name>:\tskips any database object with the given <name> that already exists" +
            "\nalter-<name>:\talters any database object with the given <name> that already exists" +
            "\ndrop-<name>:\tdrops any database object with the given <name> that already exists";
    }
}