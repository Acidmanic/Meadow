using System.IO;

// ReSharper disable once CheckNamespace
namespace System.IO
{
    public static  class DirectoryInfoExtensions
    {



        public static void CreateByParents(this DirectoryInfo directory)
        {
            if (directory==null || directory.Exists)
            {
                return;
            }
            CreateByParents(directory.Parent);
            
            directory.Create();
        }
    }
    
    
}