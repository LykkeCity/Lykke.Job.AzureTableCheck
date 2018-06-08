using Lykke.SettingsReader.Attributes;

namespace Lykke.Job.AzureTableCheck.Settings.JobSettings
{
    public class DbSettings
    {
        [AzureTableCheck]
        public string LogsConnString { get; set; }
    }
}
