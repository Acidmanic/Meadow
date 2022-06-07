using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;

namespace Meadow.Tools.Assistant.Utils
{
    public class ListFilesObserver : ISearchObserver<List<FileInfo>>
    {
        public List<FileInfo> Result { get; }

        public ListFilesObserver()
        {
            Result = new List<FileInfo>();
        }

        public void OnDirectory(DirectoryInfo dir)
        {
        }

        public virtual void OnFile(DirectoryInfo location, FileInfo file)
        {
            Result.Add(file);
        }
    }
}