using YouMap.Framework;
using mPower.Framework.Mongo;
using StructureMap;
using StructureMap.Configuration.DSL;

namespace mPower.Framework.Registries
{
    public class MongoRegistry
    {
        public MongoRegistry(IContainer container)
        {
            var settings = container.GetInstance<Settings>();

            container.Configure(config =>
            {
                // Mongo Read database
                config.For<MongoRead>().Singleton().Use(() => 
                    new MongoRead(settings.MongoReadDatabaseConnectionString));

                // Mongo Write database
                config.For<MongoWrite>().Singleton().Use(() => 
                    new MongoWrite(settings.MongoWriteDatabaseConnectionString));

                // Mongo Temporary database
                config.For<MongoTemp>().Singleton().Use(() => 
                    new MongoTemp(settings.MongoTempDatabaseConnectionString));

                
            });

            // Configure mongo driver
            MongoConvention.Configure();
        }
    }
}