using System;
using MongoDB.Bson.Serialization.Conventions;

namespace YouMap.Framework.Mongo
{
    /// <summary>
    /// Do not search for ID property
    /// </summary>
    public class NoDefaultPropertyIdConvention : IIdMemberConvention
    {
        public string FindIdMember(Type type)
        {
            return null;
        }
    }
}