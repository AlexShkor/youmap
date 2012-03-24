using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Text;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using StructureMap;
using YouMap.Framework;
using YouMap.Framework.Registries;

namespace YouMap.Admin
{
    public class DeploymentHelper
    {
        //private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        private MongoRead _read;
        private readonly IContainer _container;
        private readonly ITransitionRepository _transitionRepository;
        private readonly Settings _settings;


        public DeploymentHelper(Settings settings, MongoRead read, MongoWrite write, IContainer container, ITransitionRepository transitionRepository)
        {
            _read = read;
            _container = container;
            _transitionRepository = transitionRepository;
            _settings = settings;
        }

        public void SwitchReadMode(string url)
        {
            using (var client = new WebClient())
            {
                var enc = new UTF8Encoding();
                var data = client.DownloadData(url);
                var status = enc.GetString(data);
            }
        }

        #region Read Model Generation

        public void RegenerateReadModel(string readConnection, string writeConnection)
        {
            var sw = new Stopwatch();
            sw.Start();
            //_logger.Info("Read model regeneration started.");

            WorkWithTransitions(readConnection, writeConnection, (transitions, dispatcher, transitionRepository) =>
            {

                foreach (var transition in transitions)
                {
                    foreach (var evnt in transition.Events)
                    {
                        dispatcher.Dispatch(evnt.Data);
                    }
                }
            });
            sw.Stop();
            //_logger.Info("Read model regenerated in: " + sw.Elapsed.ToString());
        }

        public void WorkWithTransitions(string readConnection, string writeConnection, Action<List<Transition>, Dispatcher, ITransitionRepository> action)
        {
            ReconfigureMongos(readConnection, writeConnection);

            ReconfigureTransitions(writeConnection);

            _transitionRepository.EnsureIndexes();
            var transitions = _transitionRepository.GetTransitions();

            var dispatcher = Dispatcher.Create(d => d
                    .SetServiceLocator(new StructureMapServiceLocator(_container))
                );

            _read.Database.Drop();
            _read.EnsureIndexes();
            action(transitions, dispatcher, _transitionRepository);

            //restore old connection strings
            new MongoRegistry(_container);
            ReconfigureMongos(_settings.MongoReadDatabaseConnectionString, _settings.MongoWriteDatabaseConnectionString);
            ReconfigureTransitions(_settings.MongoWriteDatabaseConnectionString);
        }

        private void ReconfigureMongos(string read, string write)
        {
            //_container.Configure(config =>
            //{
            //    // Mongo Read database
            //    config.For<MongoRead>().Singleton().Use(() =>
            //                                            new MongoRead(read));

            //    // Mongo Write database
            //    config.For<MongoWrite>().Singleton().Use(() =>
            //                                            new MongoWrite(write));
            //});
            //_read = _container.GetInstance<MongoRead>();
        }

        private void ReconfigureTransitions(string writeConnectionString)
        {
            #region Transitionas Configuration

            //// 
            //// Domain and Event store configuration
            ////
            //var dataTypeRegistry = new AssemblyQualifiedDataTypeRegistry();

            //var transitionsRepository = new MongoTransitionRepository(
            //    new AssemblyQualifiedDataTypeRegistry(),
            //    writeConnectionString);

            //var transitionsStorage = new TransitionStorage(transitionsRepository);

            //_container.Configure(config =>
            //{
            //    config.For<ITransitionStorage>().Singleton().Use(transitionsStorage);
            //    config.For<ITransitionRepository>().Singleton().Use(transitionsRepository);
            //    config.For<IDataTypeRegistry>().Singleton().Use(dataTypeRegistry);
            //    config.For<IEventBus>().Use<ParalectServiceBusEventBus>();

            //    // We are using default implementation of repository
            //    config.For<IRepository>().Use<Repository>();
            //});

            #endregion
        }

        #endregion
    }
}