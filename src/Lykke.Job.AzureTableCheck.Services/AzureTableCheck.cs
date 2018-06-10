using Common.Log;
using Lykke.Job.AzureTableCheck.Core.Services;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Lykke.Job.AzureTableCheck.Services
{
    public class AzureTableCheck: IAzureTableCheck
    {
        private CloudStorageAccount account;
        private readonly ILog _log;

        public AzureTableCheck(ILog log)
        {
            _log = log ?? throw new ArgumentNullException(nameof(log));
        }

        public async Task<List<string>> GetTableNames(string connectionString)
        {
            var tableList = new List<string>();
            if (CloudStorageAccount.TryParse(connectionString, out account))
            {
                var tableClient = new CloudTableClient(account.TableEndpoint, account.Credentials);
                TableContinuationToken token = null;

                try
                {
                    do
                    {
                        var segmentedTablesList = await tableClient.ListTablesSegmentedAsync(token);
                        tableList.AddRange(segmentedTablesList.Results.Select(x => x.Name));
                        token = segmentedTablesList.ContinuationToken;
                    } while (token != null);
                }
                catch (Exception e)
                {
                    await _log.WriteErrorAsync(nameof(AzureTableCheck), $"Getting table names - account:\"{account.Credentials.AccountName}\"", e);
                }
                                
            }
            return tableList;
        }

        public async Task<int> GetNumberOfRows(string tableName, string connectionString)
        {
            var _numberOfRows = 0;
            if (CloudStorageAccount.TryParse(connectionString, out account))
            {
                var tableClient = new CloudTableClient(account.TableEndpoint, account.Credentials);
                var table = tableClient.GetTableReference(tableName);
                var tableQuery = new TableQuery();  
                
                TableContinuationToken token = null;
                try
                {
                    do
                    {
                        var queryResult = await table.ExecuteQuerySegmentedAsync(tableQuery.Select(new List<string> { "PartitionKey","RowKey","Timestamp" }), token);
                        _numberOfRows += queryResult.Results.Count;
                        token = queryResult.ContinuationToken;
                    } while (token != null);
                }                
                catch (Exception e)
                {
                    await _log.WriteErrorAsync(nameof(AzureTableCheck), $"Getting number of rows from table:\"{tableName}\"", e);
                }
            }
            return _numberOfRows;
        }

        public async Task<List<string>> GetAzureTableConnectionStrings(string apiUrl)
        {
            var azureTableList = new List<string>();
            try
            {
                var json = "";
                var request = WebRequest.Create(apiUrl) as HttpWebRequest;
                using (var response = request.GetResponse() as HttpWebResponse)
                {
                    var reader = new StreamReader(response.GetResponseStream());
                    json = reader.ReadToEnd();
                }
                var result = JArray.Parse(json);
                var azureTables = result.ToList();

                foreach (var str in azureTables)
                {
                    azureTableList.Add(str.ToString());
                }
            }
            catch (Exception e)
            {
                await _log.WriteErrorAsync(nameof(AzureTableCheck), $"Getting AzureConnectionStrings from Settings Service(API:{apiUrl})", e);
            }

            return azureTableList;
        }
    }
}
