
using KartverketWebApp.API_Models;

namespace KartverketWebApp.Services
{
    public interface ISokeService
    {
      Task<KommunerResponse> GetSokeAsync(string kommuneName);
    }
}
