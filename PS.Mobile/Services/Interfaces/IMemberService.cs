
using PS.Core.Models.ApiRequestResponse;

namespace PS.Mobile.Services.Interfaces
{
    public interface IMemberService
    {
        Task<(GetNearestStationsResponse Stations, bool success)> GetStationsAsync();
    }
}
