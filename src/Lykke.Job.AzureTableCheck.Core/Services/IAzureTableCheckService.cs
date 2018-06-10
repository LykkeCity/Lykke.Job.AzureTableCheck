using AzureStorage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.AzureTableCheck.Core.Services
{
    public interface IAzureTableCheckService
    {
        Task<List<string>> GetTableNames(string connectionString);
        Task<List<string>> GetAzureTableConnectionStrings(string apiUrl);
        Task<int> GetNumberOfRows(INoSQLTableStorage<TableEntity> tableStorage, int numberOfRetries);
    }
}
