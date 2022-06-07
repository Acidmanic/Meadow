using Meadow.Tools.Assistant.Utils.ProjectReferences;

namespace Meadow.Tools.Assistant.Compilation.ProjectReferences
{
    public class PackageReference:Reference
    {
        public PackageReference(string projectFile, string nugetName, string nugetVersion):base(projectFile)
        {
            PackageName = nugetName;

            PackageVersion = nugetVersion;

            IsPackageReference = true;
            
        }
        
    }
}