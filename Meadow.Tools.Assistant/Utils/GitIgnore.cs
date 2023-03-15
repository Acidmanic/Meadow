using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meadow.Tools.Assistant.Utils
{
    /// <summary>
    /// This class will represent a '.gitignore' file inside a directory. It can create, append item, remove item
    /// and list ignored files.
    /// </summary>
    public class GitIgnore
    {
        private readonly string _gitignoreFile;

        public GitIgnore(string directory)
        {
            directory = new DirectoryInfo(directory).FullName;

            _gitignoreFile = Path.Combine(directory, ".gitignore");
        }


        public List<string> Items()
        {
            var items = new List<string>();

            if (File.Exists(_gitignoreFile))
            {
                var lines = File.ReadAllLines(_gitignoreFile);

                foreach (var line in lines)
                {
                    var item = line.Trim();

                    if (!items.Contains(item))
                    {
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public bool ContainsNoneCommentItem(string item)
        {
            return ContainsNoneCommentItem(item, null);
        }

        private bool ContainsNoneCommentItem(string item, List<string> readItems)
        {
            if (item != null)
            {
                item = item.Trim().ToLower();

                var lines = Items();

                if (readItems != null)
                {
                    readItems.Clear();
                
                    readItems.AddRange(lines);    
                }
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        if (line == item && line == item)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public void AppendIfNotExits(string item)
        {
            var allItems = new List<string>();

            if (! ContainsNoneCommentItem(item, allItems))
            {
                allItems.Add(item);
                
                Save(allItems);
            }
            
        }

        private void Save(IEnumerable<string> items)
        {
            if (File.Exists(_gitignoreFile))
            {
                File.Delete(_gitignoreFile);
            }
            File.WriteAllLines(_gitignoreFile, items);
        }

        private bool IsCommentLine(string line)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                return false;
            }

            line = line.Trim();

            return line.StartsWith("#");
        }
    }
}