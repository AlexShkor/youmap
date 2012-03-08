using System;
using MongoDB.Driver;
using System.IO;

namespace mPower.Framework
{
    public class MPowerSettings
    {
        public string MongoWriteDatabaseConnectionString = "mongodb://localhost:27020/youmap_write1";
        public string MongoReadDatabaseConnectionString = "mongodb://localhost:27020/youmap_read1";
        public string MongoTempDatabaseConnectionString = "mongodb://localhost:27020/youmap_temp1";
        public string InputQueueName = "youmap_input_msmq";
        public string ErrorQueueName = "youmap_error_msmq";
        //{
        

    //    [SettingsProperty("mPower.Mongo.ReadDatabaseConnectionString")]
    //    public string MongoReadDatabaseConnectionString { get; set; }

    //    public MongoUrl LocalReadMongoUrl
    //    {
    //        get { return MongoUrl.Create(MongoReadDatabaseConnectionString); }
    //    }

    //    public MongoUrl LocalWriteMongoUrl
    //    {
    //        get { return MongoUrl.Create(MongoWriteDatabaseConnectionString); }
    //    }

    //    public MongoUrl LocalYodleeMongoUrl { get { return MongoUrl.Create(MongoYodleeDatabaseConnectionString); } }


    //    [SettingsProperty("mPower.Mongo.WriteDatabaseConnectionString")]
    //    public string MongoWriteDatabaseConnectionString { get; set; }

    //    [SettingsProperty("mPower.Mongo.TempDatabaseConnectionString")]
    //    public string MongoTempDatabaseConnectionString { get; set; }

    //    [SettingsProperty("mPower.Mongo.YodleeDatabaseConnectionString")]
    //    public string MongoYodleeDatabaseConnectionString { get; set; }

    //    [SettingsProperty("mPower.Mongo.IntuitDatabaseConnectionString")]
    //    public string MongoIntuitDatabaseConnectionString { get; set; }

    //    [SettingsProperty("MPower.Mongo.TestReadDatabaseConnectionString")]
    //    public string MongoTestReadDatabaseConnectionString { get; set; }

    //    [SettingsProperty("MPower.Mongo.LogsCollectionName")]
    //    public string LogsCollectionName { get; set; }

    //    [SettingsProperty("MPower.Mongo.LogsDatabaseConnectionString")]
    //    public string LogsDatabaseConnectionString { get; set; }

    //    [SettingsProperty("MPower.Chargify.ApiKey")]
    //    public string ChargifyApiKey { get; set; }

    //    [SettingsProperty("MPower.Chargify.ApiPassword")]
    //    public string ChargifyApiPassword { get; set; }

    //    [SettingsProperty("MPower.Backup.PathToBackuper")]
    //    public string PathToBackuper { get; set; }

    //    [SettingsProperty("MPower.Backup.PathToRestorer")]
    //    public string PathToRestorer { get; set; }

    //    [SettingsProperty("MPower.Backup.Password")]
    //    public string BackupDbPassword { get; set; }

    //    [SettingsProperty("MPower.Backup.UserName")]
    //    public string BackupDbUserName { get; set; }

    //    [SettingsProperty("MPower.Mongo.ReadBackupFolder")]
    //    public string ReadBackupFolder { get; set; }

    //    [SettingsProperty("MPower.Mongo.WriteBackupFolder")]
    //    public string WriteBackupFolder { get; set; }

       
        
    //    [SettingsProperty("MPower.Mongo.YodleeBackupFolder")]
    //    public string YodleeBackupFolder { get; set; }

    //    [SettingsProperty("MPower.DefaultTenantAssembly")]
    //    public string DefaultTenantAssembly { get; set; }

    //    [SettingsProperty("MPower.JanrainApiBaseUrl")]
    //    public string JanrainApiBaseUrl { get; set; }

    //    [SettingsProperty("MPower.ZillowApiBaseUrl")]
    //    public string ZillowApiBaseUrl { get; set; }

    //    [SettingsProperty("MPower.ZillowWebServiceId")]
    //    public string ZillowWebServiceId { get; set; }

    //    /// <summary>
    //    /// For tests only, because of each tenant will have his own key 
    //    /// </summary>
    //    [SettingsProperty("MPower.Membership.ApiKey")]
    //    public string MembershipApiKey { get; set; }

    //    [SettingsProperty("MPower.Membership.BaseUrl")]
    //    public string MembershipBaseUrl { get; set; }

    //    [SettingsProperty("MPower.WebHashKey")]
    //    public string WebHashKey { get; set; }

    //    [SettingsProperty("EnvironmentType")]
    //    public string EnvironmentType { get; set; }

    //    public EnvironmentTypeEnum EnvironmentTypeEnum
    //    {
    //        get { return (EnvironmentTypeEnum)Enum.Parse(typeof(EnvironmentTypeEnum), EnvironmentType); }
    //    }

    //    [SettingsProperty("Mpower.HtmlToPdfToolPath")]
    //    public string HtmlToPdfToolPath { get; set; }

    //    [SettingsProperty("Mpower.NeverLengthInYears")]
    //    public string NeverLengthInYears { get; set; }

    //    [SettingsProperty("Mpower.WhiteEmailsList")]
    //    public string WhiteEmailsList { get; set; }

    //    [SettingsProperty("Mpower.WelcomeEventText")]
    //    public string WelcomeEventText { get; set; }

    //    // Queues' Names
    //    [SettingsProperty("Mpower.InputQueueName")]
    //    public string InputQueueName { get; set; }

    //    [SettingsProperty("Mpower.ErrorQueueName")]
    //    public string ErrorQueueName { get; set; }

    //    [SettingsProperty("Mpower.Scheduler.InputQueueName")]
    //    public string SchedulerInputQueueName { get; set; }

    //    [SettingsProperty("Mpower.Scheduler.ErrorQueueName")]
    //    public string SchedulerErrorQueueName { get; set; }

    //    [SettingsProperty("Mpower.LuceneIndexesDirectory")]
    //    public string LuceneIndexesDirectory { get; set; }

    //    [SettingsProperty("Mpower.SendToErrorEmails")]
    //    public string SendToErrorEmails { get; set; }

    //    [SettingsProperty]
    //    public DeploySettings Deploy { get; set; }
    }

    public class DeploySettings
    {

        //[SettingsProperty("Deploy.Prod.YodleeConnectionString")]
        //public string ProdYodleeConnectionString { get; set; }

        //public MongoUrl ProdYodleeMongoUrl
        //{
        //    get { return MongoUrl.Create(ProdYodleeConnectionString); }
        //}

        //[SettingsProperty("Deploy.Prod.ReadModeUrl")]
        //public string ProdReadModeUrl { get; set; }

        //[SettingsProperty("Deploy.Prod.WriteConnectionString")]
        //public string ProdWriteConnectionString { get; set; }

        //public MongoUrl ProdWriteMongoUrl
        //{
        //    get { return MongoUrl.Create(ProdWriteConnectionString); }
        //}

        //[SettingsProperty("Deploy.Prod.ReadConnectionString")]
        //public string ProdReadConnectionString { get; set; }

        //public MongoUrl ProdReadMongoUrl
        //{
        //    get { return MongoUrl.Create(ProdReadConnectionString); }
        //}

        //[SettingsProperty("Deploy.Prod.ServerRootFolder")]
        //public string ProdServerRootFolder { get; set; }

        //[SettingsProperty("Deploy.Stage.BackupFolder")]
        //public string StageBackupFolder { get; set; }

        //[SettingsProperty("Deploy.Stage.PackagesFolder")]
        //public string StagePackagesFolder { get; set; }


        //[SettingsProperty("Deploy.Stage.PublisherPath")]
        //public string StagePublisherPath { get; set; }

        //[SettingsProperty("Deploy.Stage.ProdWebBuildFolder")]
        //public string StageProdWebBuildFolder { get; set; }

        //[SettingsProperty("Deploy.Stage.ServerRootFolder")]
        //public string StageServerRootFolder { get; set; }

        //public string StageReadBackupFolder
        //{
        //    get { return Path.Combine(StageBackupFolder, "read"); }
        //}

        //public string StageWriteBackupFolder
        //{
        //    get { return Path.Combine(StageBackupFolder, "write"); }
        //}

        //[SettingsProperty("Deploy.Stage.WriteConnectionString")]
        //public string StageWriteConnectionString { get; set; }

        //public MongoUrl StageWriteMongoUrl
        //{
        //    get { return MongoUrl.Create(StageWriteConnectionString); }
        //}


        //[SettingsProperty("Deploy.Stage.ReadConnectionString")]
        //public string StageReadConnectionString { get; set; }

        //public MongoUrl StageReadMongoUrl
        //{
        //    get { return MongoUrl.Create(StageReadConnectionString); }
        //}
    }
}
