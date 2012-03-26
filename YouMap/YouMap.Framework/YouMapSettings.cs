using System.Configuration;
using System.Web.Configuration;

namespace YouMap.Framework
{
    public class Settings
    {
        private static Settings _current;

        public static Settings Current {get
        {
            return _current = _current ?? new Settings();
        }}

        public string MongoWriteDatabaseConnectionString // = "mongodb://localhost:27020/youmap_write2";
        {
            get { return WebConfigurationManager.AppSettings["MongoWriteDatabaseConnectionString"]; }
        }

        public string MongoReadDatabaseConnectionString //= "mongodb://localhost:27020/youmap_read2";
        {
            get { return WebConfigurationManager.AppSettings["MongoReadDatabaseConnectionString"]; }
        }

        public string MongoTempDatabaseConnectionString //= "mongodb://localhost:27020/youmap_temp2";
        {
            get { return WebConfigurationManager.AppSettings["MongoTempDatabaseConnectionString"]; }
        }

        public string InputQueueName //= "youmap_input_msmq";
        {
            get { return WebConfigurationManager.AppSettings["InputQueueName"]; }
        }

        public string ErrorQueueName //= "youmap_error_msmq";
        {
            get { return WebConfigurationManager.AppSettings["ErrorQueueName"]; }
        }

        public string LuceneIndexesDirectory
        {
            get { return WebConfigurationManager.AppSettings["LuceneIndexesDirectory"]; }
        }

        public string VkAppId
        {
            get { return WebConfigurationManager.AppSettings["VkAppId"]; }
        }

    }

    public class DeploySettings
    {

    }
}
