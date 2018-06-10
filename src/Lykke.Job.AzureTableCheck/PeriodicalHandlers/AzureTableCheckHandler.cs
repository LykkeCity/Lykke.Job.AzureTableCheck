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
    public class AzureTableCheckHandler : TimerPeriod
    {
        private readonly IAzureTableCheckService _azureTableCheck;
        private readonly ILog _log;

        public AzureTableCheckHandler(ILog log, IAzureTableCheckService azureTableCheck) :
            base(nameof(AzureTableCheckHandler), (int)TimeSpan.FromSeconds(AppSettings.CheckPeriodInSeconds).TotalMilliseconds, log)
        {
            _log = log;
            _azureTableCheck = azureTableCheck;
        }

        public override async Task Execute()
        {
            var azureTableList = await _azureTableCheck.GetAzureTableConnectionStrings(AppSettings.SettingsApiLink);            

            foreach (var azureStorage in azureTableList)
            {
                var tableList = await _azureTableCheck.GetTableNames(azureStorage);

                if (tableList.Count != 0)
                {
                    var account = CloudStorageAccount.Parse(azureStorage);
                    await _log.WriteMonitorAsync(nameof(AzureTableCheckHandler),"Check started", $"Checking storage \"{account.Credentials.AccountName}\" STARTED.");           

                    var badTables = new List<string>();

                    foreach (var tableName in tableList)
                    {
                        var rowsInTable = await _azureTableCheck.GetNumberOfRows(tableName, azureStorage);
                        if (rowsInTable > AppSettings.MaxEntitiesInOneTable)
                        {                      
                            badTables.Add(tableName);
                        }
                        //await _log.WriteInfoAsync(nameof(AzureTableCheckHandler), "Check table", $"Check table {tableName} FINISHED. Size: {rowsInTable}.");
                    }

                    if (badTables.Count != 0)
                    {
                        var badTablesStr = "";

                        foreach(var name in badTables)
                        {
                            if(name == badTables.Last())
                            {
                                badTablesStr += $"{name}.";
                            }
                            else
                            {
                                badTablesStr += $"{name}, ";
                            }
                            
                        }
                        await _log.WriteMonitorAsync(nameof(AzureTableCheckHandler), "Check finished", $"Checking storage \"{account.Credentials.AccountName}\" FINISHED. Partitions total: {tableList.Count}. Partitions with size more than {AppSettings.MaxEntitiesInOneTable}: {badTablesStr} ");
                    }
                    else
                    {
                        await _log.WriteMonitorAsync(nameof(AzureTableCheckHandler), "Check finished", $"Checking storage \"{account.Credentials.AccountName}\" FINISHED. Partitions total: {tableList.Count}. There are no partitions with size more than {AppSettings.MaxEntitiesInOneTable}.");
                    }

                }


            }        
            
            await Task.CompletedTask;
        }        
    }
}
