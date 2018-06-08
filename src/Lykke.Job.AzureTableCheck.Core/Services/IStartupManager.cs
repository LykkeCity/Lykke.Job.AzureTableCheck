using System.Threading.Tasks;

namespace Lykke.Job.AzureTableCheck.Core.Services
{
    public interface IStartupManager
    {
        Task StartAsync();
    }
}