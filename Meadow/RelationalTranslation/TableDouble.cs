using System;
using System.Collections.Generic;

namespace Meadow.RelationalTranslation
{
    public class TableDouble
    {
        public string Name { get; set; }
        
        public Type DataType { get; set; }
        
        public List<string> Fields { get; set; }
        
        public List<string> Joins { get; set; }
        
        public string From { get; set; }
        
    }
}