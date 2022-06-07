using System.IO;
using Meadow.Tools.Assistant.Utils.ProjectReferences;

namespace Meadow.Tools.Assistant.Compilation.ProjectReferences
{
    public class ProjectReference : Reference
    {
        public ProjectReference(string ownerProjectFile, string referencedProjectFile) : base(ownerProjectFile)
        {
            IsProjectReference = true;

            ReferenceProjectFile = new FileInfo(referencedProjectFile);
        }

        public ProjectReference(string ownerProjectFile, FileInfo referencedProjectFile) : base(ownerProjectFile)
        {
            IsProjectReference = true;

            ReferenceProjectFile = referencedProjectFile;
        }
    }
}