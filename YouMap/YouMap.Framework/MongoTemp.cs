using System;
using MongoDB.Driver;

namespace YouMap.Framework
{
    public class MongoTemp
    {
        /// <summary>
        /// MongoDB Server
        /// </summary>
        private readonly MongoServer _server;

        /// <summary>
        /// Name of database 
        /// </summary>
        private readonly string _databaseName;

        /// <summary>
        /// Opens connection to MongoDB Server
        /// </summary>
        public MongoTemp(String connectionString)
        {
            _databaseName = MongoUrl.Create(connectionString).DatabaseName;
            _server = MongoServer.Create(connectionString);
        }

        /// <summary>
        /// MongoDB Server
        /// </summary>
        public MongoServer Server
        {
            get { return _server; }
        }

        /// <summary>
        /// Get database
        /// </summary>
        public MongoDatabase Database
        {
            get { return _server.GetDatabase(_databaseName); }
        }

        public MongoCollection Imports
        {
            get { return Database.GetCollection("imports"); }
        }

        public MongoCollection Alerts
        {
            get { return Database.GetCollection("alerts"); }
        }

        public MongoCollection Notifications
        {
            get { return Database.GetCollection("notifications"); }
        }

        public MongoCollection CommandLogs
        {
            get { return Database.GetCollection("command_logs"); }
        }
    }
}
