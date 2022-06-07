using System.IO;

namespace Meadow.Tools.Assistant.Utils
{
    public interface ISearchObserver<TOutput>
    {
        TOutput Result { get; }

        void OnDirectory(DirectoryInfo dir);

        void OnFile(DirectoryInfo location, FileInfo file);
    }
}