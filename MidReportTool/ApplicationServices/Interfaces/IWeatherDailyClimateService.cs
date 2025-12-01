using MidReportTool.Api.Models;
using MidReportTool.Api.Responses;
using MidReportTool.ApplicationServices.Models;
using MidReportTool.Domain;

namespace MidReportTool.ApplicationServices.Interfaces
{
    public interface IWeatherDailyClimateService
    {
        Task<Result> UploadAsync(UploadWeatherDailyClimateChangesDTO dto);
         Task<ResultT<List<WeatherDailyClimateDto>>> GetAllRecordsAsync();
        Task<ResultT<List<WeatherDailyClimateDto>>> GetSpecificRecordsByCityAsync(string city);
        Task<ResultT<List<WeatherDailyClimateDto>>> GetFilteredRecordsAsyncV1(WeatherFilterRequest request);


    }
}
