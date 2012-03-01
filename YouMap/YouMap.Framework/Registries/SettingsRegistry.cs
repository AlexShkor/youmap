
using StructureMap;
using StructureMap.Configuration.DSL;

namespace mPower.Framework.Registries
{
    public class SettingsRegistry
    {
        public SettingsRegistry(IContainer container)
        {
            var settings = new MPowerSettings();
            
            container.Configure(config => config.ForSingletonOf<MPowerSettings>().Use(settings));
        }
    }
}