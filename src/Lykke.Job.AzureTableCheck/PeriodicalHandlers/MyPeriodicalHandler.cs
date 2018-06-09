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
            var azureTableList = _azureTableCheck.GetAzureTableConnectionStrings(AppSettings.SettingsApiLink);            

            foreach (var azureStorage in azureTableList)
            {
                var tableList = await _azureTableCheck.GetTablesNameForAzureSubscription(azureStorage);

                if (tableList.Count != 0)
                {
                    var account = CloudStorageAccount.Parse(azureStorage);
                    Console.WriteLine($"Checking storage \"{account.Credentials.AccountName}\" STARTED.");                

                    var badTables = new List<string>();

                    foreach (var tableName in tableList)
                    {
                        var rowsInTable = await _azureTableCheck.NumberOfRows(tableName, azureStorage);
                        if (rowsInTable > AppSettings.MaxEntitiesInOneTable)
                        {
                            //TODO: Send messages about problem                       
                            badTables.Add(tableName);
                        }
                    }

                    if (badTables.Count != 0)
                    {
                        var badTablesStr = "";

                        foreach(var name in badTables)
                        {
                            badTablesStr += $"{name}, ";
                        }
                        Console.WriteLine($"Checking storage \"{account.Credentials.AccountName}\" FINISHED. Partitions total: {tableList.Count}. Partitions with size more than {AppSettings.MaxEntitiesInOneTable}: {badTablesStr} ");
                    }
                    else
                    {
                        Console.WriteLine($"Checking storage \"{account.Credentials.AccountName}\" FINISHED. Partitions total: {tableList.Count}. There are no partitions with size more than {AppSettings.MaxEntitiesInOneTable}.");
                    }

                }


            }        
            
            await Task.CompletedTask;
        }        
    }
}
