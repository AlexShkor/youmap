using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace YouMap.Framework
{
    public class MongoRead
    {
        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of database 
        /// </summary>
        private readonly string _databaseName;

        public MongoUrl MongoUrl { get; private set; }

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongoRead(String connectionString)
        {
            MongoUrl = MongoUrl.Create(connectionString);
            _databaseName = MongoUrl.DatabaseName;
            _server = MongoServer.Create(connectionString);
        }

        public void EnsureIndexes()
        {
            GetCollection("places").EnsureIndex(IndexKeys.GeoSpatial("Location"));
            GetCollection("feeds").EnsureIndex(IndexKeys.Ascending("Name"));
        }

        /// <summary>
        /// MongoDB Server
        /// </summary>
        public MongoServer Server
        {
            get { return _server; }
        }

        #region Databases

        /// <summary>
        /// Get database
        /// </summary>
        public MongoDatabase Database
        {
            get { return _server.GetDatabase(_databaseName); }
        }

        public List<string> AllDatabases()
        {
            return _server.GetDatabaseNames().ToList();
        }

        public MongoDatabase GetDatabase(string name)
        {
            return _server.GetDatabase(name);
        }

        #endregion

        #region Collections

        /// <summary>
        /// Membership users collection
        /// </summary>
        public MongoCollection Users
        {
            get { return Database.GetCollection("users"); }
        }

        public MongoCollection UserLogins
        {
            get { return Database.GetCollection("user_logins"); }
        }

        public MongoCollection Test
        {
            get { return Database.GetCollection("test"); }
        }

        public MongoCollection EventLogs
        {
            get { return Database.GetCollection("event_logs"); }
        }

        public MongoCollection GetCollection(string name)
        {
            return Database.GetCollection(name);
        }

        #endregion

    }
}
