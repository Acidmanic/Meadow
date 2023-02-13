using CoreCommandLine;

namespace Meadow.Tools.Assistant
{
    public static class ContextExtensions
    {
        public static readonly string TargetProjectPathKey = "TargetProjectPathKey";
        public static readonly string ScriptsDirectoryPathKey = "ScriptsDirectoryPathKey";


        public static string GetTargetProjectPath(this Context context)
        {
            return context.Get<string>(TargetProjectPathKey, ".");
        }

        public static void SetTargetProjectPath(this Context context, string path)
        {
            context.Set(TargetProjectPathKey, path);
        }
     public static string GetScriptsDirectoryPath(this Context context)
        {
            return context.Get<string>(ScriptsDirectoryPathKey, null);
        }

        public static void SetScriptsDirectoryPath(this Context context, string path)
        {
            context.Set(ScriptsDirectoryPathKey, path);
        }
    }
}