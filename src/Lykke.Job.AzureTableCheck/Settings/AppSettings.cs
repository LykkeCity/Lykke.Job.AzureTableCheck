using Lykke.Job.AzureTableCheck.Settings.JobSettings;
using Lykke.Job.AzureTableCheck.Settings.SlackNotifications;
using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.AzureTableCheck.Settings
{
    public class AppSettings
    {
        public AzureTableCheckSettings AzureTableCheckJob { get; set; }

        public static string SettingsApiLink { get; set; }

        public static int CheckPeriodInSeconds { get; set; }

        public SlackNotificationsSettings SlackNotifications { get; set; }



        [Optional]
        public MonitoringServiceClientSettings MonitoringServiceClient { get; set; }
    }
}
