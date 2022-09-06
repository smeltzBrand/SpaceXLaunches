using SpaceXLaunches.Models;

namespace SpaceXLaunches.Services.Interfaces
{
    public interface ILaunchJSONService
    {
        public Task<List<Launch>> GetLaunchDataAsync();

        public Task<List<Payload>> GetPayloadRankAsync();
    }
}
