using Microsoft.Extensions.DependencyInjection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Log;
using Lykke.Job.AzureTableCheck.Core.Services;
using Lykke.Job.AzureTableCheck.Settings.JobSettings;
using Lykke.Job.AzureTableCheck.Services;
using Lykke.SettingsReader;
using Lykke.Job.AzureTableCheck.PeriodicalHandlers;

namespace Lykke.Job.AzureTableCheck.Modules
{
    public class JobModule : Module
    {
        private readonly AzureTableCheckSettings _settings;
        private readonly IReloadingManager<AzureTableCheckSettings> _settingsManager;
        private readonly ILog _log;
        // NOTE: you can remove it if you don't need to use IServiceCollection extensions to register service specific dependencies
        private readonly IServiceCollection _services;

        public JobModule(AzureTableCheckSettings settings, IReloadingManager<AzureTableCheckSettings> settingsManager, ILog log)
        {
            _settings = settings;
            _log = log;
            _settingsManager = settingsManager;

            _services = new ServiceCollection();
        }

        protected override void Load(ContainerBuilder builder)
        {
            // NOTE: Do not register entire settings in container, pass necessary settings to services which requires them
            // ex:
            // builder.RegisterType<QuotesPublisher>()
            //  .As<IQuotesPublisher>()
            //  .WithParameter(TypedParameter.From(_settings.Rabbit.ConnectionString))

            builder.RegisterInstance(_log)
                .As<ILog>()
                .SingleInstance();

            builder.RegisterType<HealthService>()
                .As<IHealthService>()
                .SingleInstance();

            builder.RegisterType<StartupManager>()
                .As<IStartupManager>();

            builder.RegisterType<ShutdownManager>()
                .As<IShutdownManager>();

            builder.RegisterType<Services.AzureTableCheck>()
                .As<IAzureTableCheck>();

            RegisterPeriodicalHandlers(builder);

            // TODO: Add your dependencies here

            builder.Populate(_services);
        }

        private void RegisterPeriodicalHandlers(ContainerBuilder builder)
        {
            // TODO: You should register each periodical handler in DI container as IStartable singleton and autoactivate it

            builder.RegisterType<AzureTableCheckHandler>()
                .As<IStartable>()
                .AutoActivate()
                .SingleInstance();
        }
    }
}
