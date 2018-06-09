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

        public async Task<List<string>> GetTablesNameForAzureSubscription(string connectionString)
        {
            var tableList = new List<string>();
            if (CloudStorageAccount.TryParse(connectionString, out account))
            {
                var tableClient = new CloudTableClient(account.TableEndpoint, account.Credentials);
                TableContinuationToken token = null;

                do
                {
                    var segmentedTablesList = await tableClient.ListTablesSegmentedAsync(token);
                    tableList.AddRange(segmentedTablesList.Results.Select(x => x.Name));
                    token = segmentedTablesList.ContinuationToken;
                } while (token != null);                
            }
            return tableList;
        }

        public async Task<int> NumberOfRows(string tableName, string connectionString)
        {
            var _numberOfRows = 0;
            if (CloudStorageAccount.TryParse(connectionString, out account))
            {
                var tableClient = new CloudTableClient(account.TableEndpoint, account.Credentials);
                var table = tableClient.GetTableReference(tableName);
                TableContinuationToken token = null;
                
                do
                {
                    var queryResult = await table.ExecuteQuerySegmentedAsync(new TableQuery(), token);
                    _numberOfRows += queryResult.Count();
                    token = queryResult.ContinuationToken;
                } while (token != null);
            }
            return _numberOfRows;
        }

        public List<string> GetAzureTableConnectionStrings(string apiUrl)
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
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            return azureTableList;
            // Parse the connection string and return a reference to the storage account.


        }
    }
}
