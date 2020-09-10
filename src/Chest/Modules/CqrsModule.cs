using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Chest.Projections;
using Chest.Settings;
using Common.Log;
using Lykke.Cqrs;
using Lykke.Cqrs.Configuration;
using Lykke.Cqrs.Configuration.BoundedContext;
using Lykke.Cqrs.Configuration.Routing;
using Lykke.Cqrs.Middleware.Logging;
using Lykke.Messaging;
using Lykke.Messaging.Contract;
using Lykke.Messaging.RabbitMq;
using Lykke.Messaging.Serialization;
using MarginTrading.AssetService.Contracts.Currencies;

namespace Chest.Modules
{
    internal class CqrsModule : Module
    {
        private const string DefaultRoute = "self";
        private const string DefaultPipeline = "commands";
        private const string DefaultEventPipeline = "events";
        private readonly long _defaultRetryDelayMs;
        private readonly ILog _log;
        private readonly CqrsSettings _settings;
        private readonly CqrsContextNamesSettings _contextNames;

        public CqrsModule(CqrsSettings settings, ILog log)
        {
            _settings = settings;
            _log = log;
            _contextNames = settings.ContextNames;
            _defaultRetryDelayMs = (long) _settings.RetryDelay.TotalMilliseconds;
        }
        
        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(context => new AutofacDependencyResolver(context)).As<IDependencyResolver>()
                .SingleInstance();
            
            var rabbitMqSettings = new RabbitMQ.Client.ConnectionFactory
            {
                Uri = new Uri(_settings.ConnectionString, UriKind.Absolute)
            };
            var messagingEngine = new MessagingEngine(_log,
                new TransportResolver(new Dictionary<string, TransportInfo>
                {
                    {
                        "RabbitMq",
                        new TransportInfo(rabbitMqSettings.Endpoint.ToString(), rabbitMqSettings.UserName,
                            rabbitMqSettings.Password, "None", "RabbitMq")
                    }
                }),
                new RabbitMqTransportFactory());
            
            builder.RegisterAssemblyTypes(GetType().Assembly).Where(t =>
                new[] {"Saga", "CommandsHandler", "Projection"}.Any(ending => t.Name.EndsWith(ending))).AsSelf();
            
            builder.Register(ctx => CreateEngine(ctx, messagingEngine))
                .As<ICqrsEngine>()
                .SingleInstance()
                .AutoActivate();
        }
        
        private CqrsEngine CreateEngine(IComponentContext ctx, IMessagingEngine messagingEngine)
        {
            var rabbitMqConventionEndpointResolver = new RabbitMqConventionEndpointResolver(
                "RabbitMq",
                SerializationFormat.MessagePack,
                environment: _settings.EnvironmentName);
            
            var engine = new CqrsEngine(
                _log,
                ctx.Resolve<IDependencyResolver>(),
                messagingEngine,
                new DefaultEndpointProvider(),
                true,
                Register.DefaultEndpointResolver(rabbitMqConventionEndpointResolver),
                RegisterContext(),
                Register.CommandInterceptors(new DefaultCommandLoggingInterceptor(_log)),
                Register.EventInterceptors(new DefaultEventLoggingInterceptor(_log)));
            
            engine.StartSubscribers();

            return engine;
        }

        private IRegistration RegisterContext()
        {
            var contextRegistration = Register.BoundedContext(_contextNames.Chest)
                .FailedCommandRetryDelay(_defaultRetryDelayMs)
                .ProcessingOptions(DefaultRoute).MultiThreaded(8).QueueCapacity(1024);
            
            RegisterCurrenciesProjection(contextRegistration);
            
            return contextRegistration;
        }

        private void RegisterCurrenciesProjection(ProcessingOptionsDescriptor<IBoundedContextRegistration> contextRegistration)
        {
            contextRegistration.ListeningEvents(
                    typeof(CurrencyChangedEvent))
                .From(_settings.ContextNames.AssetService).On(DefaultEventPipeline)
                .WithProjection(
                    typeof(CurrencyProjection), _settings.ContextNames.AssetService);
        }
    }
}