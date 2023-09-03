using System;
using System.Security.Policy;
using CoreCommandLine;
using Meadow.Scaffolding.Macros;

namespace Meadow.Tools.Assistant
{
    public static class ContextExtensions
    {
        public static readonly string TargetProjectPathKey = "TargetProjectPathKey";
        public static readonly string CallModeKey = "ExternalToolCallMode";
        public static readonly string ScriptsDirectoryPathKey = "ScriptsDirectoryPathKey";
        public static readonly string MeadowConfigurationProviderTypeNameKey = "MeadowConfigurationProviderTypeName";


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

        public static void SetMeadowConfigurationProvideTypeName(this Context context, string fullname)
        {
            context.Set(MeadowConfigurationProviderTypeNameKey,fullname);
        }
        
        public static string GetMeadowConfigurationProvideTypeName(this Context context)
        {
            return context.Get(MeadowConfigurationProviderTypeNameKey,"");
        }

        public static void SetCallMode(this Context context, ExternalToolCallMode callMode)
        {
            context.Set(CallModeKey,callMode);
        }

        public static ExternalToolCallMode GetCallMode(this Context context)
        {
            return context.Get(CallModeKey, ExternalToolCallMode.Regular);
        }
    }
}