
using PS.Core.Models.ApiRequestResponse;

namespace PS.Mobile.Services.Interfaces
{
    public interface IMemberService
    {
        Task<(List<StationLite> Stations, bool success)> GetStationsAsync();
    }
}
