using System;
using System.Web.Mvc;
using System.Web.Routing;
using Paralect.Domain;
using Paralect.Domain.EventBus;
using Paralect.ServiceBus;
using Paralect.ServiceBus.Dispatching;
using Paralect.ServiceLocator.StructureMap;
using Paralect.Transitions;
using Paralect.Transitions.Mongo;
using StructureMap;
using YouMap.Admin;
using YouMap.Domain;
using YouMap.EventHandlers;
using YouMap.Framework;
using YouMap.Framework.Environment;
using YouMap.Framework.Registries;
using IIdGenerator = YouMap.Framework.Environment.IIdGenerator;

namespace YouMap
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Map", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );
            routes.MapRoute(
                "MapIndex",
                "{controller}/{action}/Place/{placeId}/Event/{eventId}",
                new
                    {
                        controller = "Map",
                        action = "Index",
                        placeId = UrlParameter.Optional,
                        eventId = UrlParameter.Optional
                    });

        }

        //Implement an Authentication Request Handler to Construct
        // a GenericPrincipal Object
        //protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        //{
        //    HttpCookie authCookie = HttpContext.Current.Request.Cookies[FormsAuthentication.FormsCookieName];
        //    if (authCookie != null)
        //    {
        //        var authTicket =
        //               FormsAuthentication.Decrypt(authCookie.Value);
        //        var query = authTicket.UserData;
        //        var identity = new UserIdentity();
        //        var user = new UserPrincipal(identity);
        //        Context.User = user;
        //    }
        //}


        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            var container = new Container();
            DependencyResolver.SetResolver(new SmDependencyResolver(container));
            #region Common Configuration

            new SettingsRegistry(container);
            new MongoRegistry(container);

            var settings = container.GetInstance<Settings>();

            //var encrypt = new EncryptionService();

            // Id generator & Membership API
            container.Configure(config =>
            {
                config.For<IIdGenerator>().Use<MongoObjectIdGenerator>();
                config.For<IContainer>().Use(container);
                //config.For<MembershipApiService>().Add(
                //    new MembershipApiService(tenant.MembershipApiKey,
                //                                settings.MembershipBaseUrl));
                // config.For<IEncryptionService>().Singleton().Use(encrypt);
                //config.For<IEmailHtmlBuilder>().Use<NuggetHtmlBuilder>();
                //config.For<ITransUnionService>().Use<TransUnionService>();
                //config.For<IIntuitService>().Use<IntuitService>();
            });

            //
            // Service bus
            //
            var bus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(container))
                .MemorySynchronousTransport()
                .SetName("Main YouMap Service Bus")
                .SetInputQueue("youmap.sync.server")
                .AddEndpoint(type => type.FullName.EndsWith("Event"), "youmap.sync.server")
                .AddEndpoint(type => type.FullName.EndsWith("Command"), "youmap.sync.server")
                .AddEndpoint(type => type.FullName.EndsWith("Message"), "youmap.sync.server")
                .Dispatcher(d => d
                    .AddHandlers(typeof(PlaceAR).Assembly) //mPower.Domain command handler
                     .AddHandlers(typeof(PlaceDocumentEventHandler).Assembly)
                   // .AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly, new[] { "mPower.EventHandlers.Immediate" }) //sync event handlers
                 )
            );

            container.Configure(config => config.For<IServiceBus>().Singleton().Use(bus));

            var asyncBus = ServiceBus.Run(c => c
                .SetServiceLocator(new StructureMapServiceLocator(container))
                .MsmqTransport()
                .SetName("Async YouMap Service Bus")
                .SetInputQueue(String.Format("{0}_{1}", settings.InputQueueName, ApplicationName))
                .SetErrorQueue(String.Format("{0}_{1}", settings.ErrorQueueName, ApplicationName))
                .AddEndpoint(type => type.FullName.EndsWith("Event"), String.Format("{0}_{1}", settings.InputQueueName, ApplicationName))
                .AddEndpoint(type => type.FullName.EndsWith("Command"), String.Format("{0}_{1}", settings.InputQueueName, ApplicationName))
                .AddEndpoint(type => type.FullName.EndsWith("Message"), String.Format("{0}_{1}", settings.InputQueueName, ApplicationName))
                .Dispatcher(d => d
                            .AddHandlers(typeof(PlaceAR).Assembly)
                            //.AddHandlers(typeof(PlaceDocumentEventHandler).Assembly)
                            //.AddHandlers(typeof(NotificationTempService).Assembly)
                            //.AddHandlers(typeof(CreditIdentityAlertDocumentEventHandler).Assembly, new[] { "mPower.EventHandlers.Eventual" }) //async event handlers
                )
            );

            container.Configure(config => config.For<AsyncServiceBus>().Singleton().Use(new AsyncServiceBus(asyncBus)));

            // 
            // Domain and Event store configuration
            //
            var dataTypeRegistry = new AssemblyQualifiedDataTypeRegistry();

            var transitionsRepository = new MongoTransitionRepository(
                new AssemblyQualifiedDataTypeRegistry(),
                settings.MongoWriteDatabaseConnectionString);

            var transitionsStorage = new TransitionStorage(transitionsRepository);

            container.Configure(config =>
            {
                config.For<ITransitionRepository>().Singleton().Use(transitionsRepository);
                config.For<ITransitionStorage>().Singleton().Use(transitionsStorage);
                config.For<IDataTypeRegistry>().Singleton().Use(dataTypeRegistry);
                config.For<IEventBus>().Use<DualEventBus>();
                config.For<IRepository>().Use<Repository>();
                config.For<IEventService>().Use<EventService>();
                config.For<ICommandService>().Use<CommandService>();
                config.For<ISessionContext>().Singleton().Use<SessionContext>();
                config.For<IAuthenticationService>().Use<AuthenticationService>();
                config.For<DeploymentHelper>().Use<DeploymentHelper>();
            });

            #endregion

        }

        protected object ApplicationName { get { return "YouMapMongo"; } }
    }
}