using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;

namespace YouMap.Framework.Mongo
{
    public static class MongoConvention
    {
        public static void Configure()
        {
            // Register bson serializer conventions
            DateTimeSerializationOptions.Defaults = DateTimeSerializationOptions.LocalInstance;
            var myConventions = new ConventionProfile();
            myConventions.SetIdMemberConvention(new NoDefaultPropertyIdConvention());
            BsonClassMap.RegisterConventions(myConventions, t => true);
        }
    }
}