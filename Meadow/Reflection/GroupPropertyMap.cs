using System;
using System.Collections.Generic;

namespace Meadow.Reflection
{
    public class GroupPropertyMap
    {
        public Dictionary<Type, List<string>> Respectives { get; set; }
        public List<string> Common { get; set; }

        public GroupPropertyMap()
        {
            Respectives = new Dictionary<Type, List<string>>();
            
            Common = new List<string>();
        }
    }
}