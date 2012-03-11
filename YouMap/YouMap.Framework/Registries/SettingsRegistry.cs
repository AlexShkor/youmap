using StructureMap;

namespace YouMap.Framework.Registries
{
    public class SettingsRegistry
    {
        public SettingsRegistry(IContainer container)
        {
            var settings = new Settings();
            
            container.Configure(config => config.ForSingletonOf<Settings>().Use(settings));
        }
    }
}