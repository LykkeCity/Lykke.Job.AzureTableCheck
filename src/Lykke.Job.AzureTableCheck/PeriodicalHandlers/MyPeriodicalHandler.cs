using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.AzureTableCheck.Settings;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Lykke.Job.AzureTableCheck.Core.Services;
using System.Net;
using System.IO;
using Newtonsoft.Json.Linq;

namespace Lykke.Job.AzureTableCheck.PeriodicalHandlers
{
    public class MyPeriodicalHandler : TimerPeriod
    {
        private readonly IAzureTableCheck _azureTableCheck;
        private readonly ILog _log;

        public MyPeriodicalHandler(ILog log, IAzureTableCheck azureTableCheck) :
            // TODO: Sometimes, it is enough to hardcode the period right here, but sometimes it's better to move it to the settings.
            // Choose the simplest and sufficient solution
            base(nameof(MyPeriodicalHandler), (int)TimeSpan.FromSeconds(AppSettings.CheckPeriodInSeconds).TotalMilliseconds, log)
        {
            _log = log;
            _azureTableCheck = azureTableCheck;
        }

        public override async Task Execute()
        {
            // TODO: Orchestrate execution flow here and delegate actual business logic implementation to services layer
            // Do not implement actual business logic here

            var azureTableList = _azureTableCheck.GetAzureTableConnectionStrings(AppSettings.SettingsApiLink);            

            foreach (var str in azureTableList)
            {
                Console.WriteLine("Connection string>" + str);
                var t = await _azureTableCheck.GetTablesNameForAzureSubscription(str.ToString());

                foreach (var tt in t)
                {
                    Console.WriteLine("---Table>" + tt);
                    Console.WriteLine("---Number of rows>" + await _azureTableCheck.NumberOfRows(tt, str.ToString()));
                }
            }

            Console.WriteLine("DONE! Repeat after - " + AppSettings.CheckPeriodInSeconds + " seconds");
            

            await Task.CompletedTask;
        }        
    }
}
