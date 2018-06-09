using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.AzureTableCheck.Core.Services
{
    public interface IAzureTableCheck
    {
        Task<List<string>> GetTablesNameForAzureSubscription(string connectionString);
        Task<int> NumberOfRows(string tableName, string connectionString);
        List<string> GetAzureTableConnectionStrings(string apiUrl);
    }
}
