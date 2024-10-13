using Microsoft.AspNetCore.Mvc;
using KartverketWebApp.API_Models;

namespace KartverketWebApp.Services
{
    public interface IStednavn
    {
        Task<StednavnResponse> GetStednavnAsync(double nord, double ost, int koordsys);
    }
}
