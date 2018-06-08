using System.Threading.Tasks;

namespace Lykke.Job.AzureTableCheck.Core.Services
{
    public interface IShutdownManager
    {
        Task StopAsync();
    }
}
