using StructureMap;
using YouMap.Framework.Mongo;

namespace YouMap.Framework.Registries
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
                                                            {
                                                                var MongoRead =
                                                                    new MongoRead(settings.MongoReadDatabaseConnectionString);
                                                                MongoRead.EnsureIndexes();
                                                                return MongoRead;
                                                            });

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