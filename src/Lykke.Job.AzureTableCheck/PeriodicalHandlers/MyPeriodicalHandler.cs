using System;
using System.Threading.Tasks;
using Common;
using Common.Log;
using Lykke.Job.AzureTableCheck.Settings;
using System.Linq;
using System.Collections.Generic;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace Lykke.Job.AzureTableCheck.PeriodicalHandlers
{
    public class MyPeriodicalHandler : TimerPeriod
    {
        public MyPeriodicalHandler(ILog log) :
            // TODO: Sometimes, it is enough to hardcode the period right here, but sometimes it's better to move it to the settings.
            // Choose the simplest and sufficient solution
            base(nameof(MyPeriodicalHandler), (int)TimeSpan.FromSeconds(AppSettings.CheckPeriodInSeconds).TotalMilliseconds, log)
        {
        }

        public override async Task Execute()
        {
            // TODO: Orchestrate execution flow here and delegate actual business logic implementation to services layer
            // Do not implement actual business logic here
            //try
            //{
            //    var request = WebRequest.Create(AppSettings.SettingsApiLink) as HttpWebRequest;
            //    var json = "";
            //    using (var response = request.GetResponse() as HttpWebResponse)
            //    {
            //        var reader = new StreamReader(response.GetResponseStream());
            //        json = reader.ReadToEnd();
            //    }

            //    var result = JArray.Parse(json);
            //    var azureTableList = result.ToList();

            //    foreach (var str in azureTableList)
            //    {
            //        Console.WriteLine(str);
            //    }

            //    Console.WriteLine("Repeat after - " + AppSettings.CheckPeriodInSeconds + " seconds");
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine(ex);
            //    await Task.CompletedTask;
            //}

            // Parse the connection string and return a reference to the storage account.

            var t = await GetTablesNameForAzureSubscription("DefaultEndpointsProtocol=https;AccountName=lkedevsettings;AccountKey=Ztpq2z5ieCo7H5Yp4GUJpWXmIqTrXe25dkJBmlnBp0g8IfrRaVV4H67EjbAFjNC8kbZEMU0TvkFGsMRVrFuvXQ==");

            foreach(var tt in t)
            {
                Console.WriteLine(tt);
                Console.WriteLine(await NumberOfRows(tt, "DefaultEndpointsProtocol=https;AccountName=lkedevsettings;AccountKey=Ztpq2z5ieCo7H5Yp4GUJpWXmIqTrXe25dkJBmlnBp0g8IfrRaVV4H67EjbAFjNC8kbZEMU0TvkFGsMRVrFuvXQ=="));
            }

            await Task.CompletedTask;
        }

        private static async Task<List<string>> GetTablesNameForAzureSubscription(string connectionString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = new CloudTableClient(account.TableEndpoint, account.Credentials);
            TableContinuationToken token = null;
            var tableList = new List<string>();
            do
            {
                var segmentedTablesList = await tableClient.ListTablesSegmentedAsync(token);
                tableList.AddRange(segmentedTablesList.Results.Select(x => x.Name));
                token = segmentedTablesList.ContinuationToken;
            } while (token != null);

            return tableList;
        }

        private static async Task<int> NumberOfRows(string tableName, string connectionString)
        {
            CloudStorageAccount account = CloudStorageAccount.Parse(connectionString);
            CloudTableClient tableClient = new CloudTableClient(account.TableEndpoint, account.Credentials);
            var table = tableClient.GetTableReference(tableName);
            TableContinuationToken token = null;
            var _numberOfRows = 0;
            do
            {
                var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery(), token);
                _numberOfRows += queryResult.Count();
                token = queryResult.ContinuationToken;
            } while (token != null);
            return _numberOfRows;
        }
    }
}
