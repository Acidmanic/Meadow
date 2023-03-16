using System.Collections.Generic;
using System.Reflection;

namespace Meadow.Configuration
{
    public class MeadowConfiguration : MeadowConfigurationModel
    {
        public List<Assembly> MacroContainingAssemblies { get; set; } = new List<Assembly>();
    }
}