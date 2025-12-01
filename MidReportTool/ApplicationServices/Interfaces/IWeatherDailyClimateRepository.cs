using MidReportTool.ApplicationServices.Models;
using MidReportTool.Domain;

namespace MidReportTool.ApplicationServices.Interfaces
{
    public interface IWeatherDailyClimateRepository
    {
        Task AddAsync(WeatherDailyClimate entity);
        Task AddRangeAsync(IEnumerable<WeatherDailyClimate> entities);
        Task<IEnumerable<WeatherDailyClimate>> GetAllAsync();

        Task<List<WeatherDailyClimate>> GetSpecificAsync(string city);
        Task<List<WeatherDailyClimate>> GetFilteredRecordsAsyncV1(WeatherFilterCriteria filter);

    }
}
