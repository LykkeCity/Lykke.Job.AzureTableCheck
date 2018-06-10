using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Lykke.Job.AzureTableCheck.Core.Services
{
    public interface IAzureTableCheck
    {
        Task<List<string>> GetTableNames(string connectionString);
        Task<int> GetNumberOfRows(string tableName, string connectionString);
        Task<List<string>> GetAzureTableConnectionStrings(string apiUrl);
    }
}
