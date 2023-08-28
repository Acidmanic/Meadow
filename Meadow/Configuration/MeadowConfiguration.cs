using System.Collections.Generic;
using System.Reflection;
using Acidmanic.Utilities.Reflection.ObjectTree;

namespace Meadow.Configuration
{
    public class MeadowConfiguration : MeadowConfigurationModel
    {
        public List<Assembly> MacroContainingAssemblies { get; set; } = new List<Assembly>();

        public IDataOwnerNameProvider TableNameProvider { get; set; } = new PluralDataOwnerNameProvider();
    }
}