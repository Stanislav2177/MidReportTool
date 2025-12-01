using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MidReportTool.Api.Models;
using MidReportTool.Api.Responses;
using MidReportTool.ApplicationServices.Interfaces;
using MidReportTool.ApplicationServices.Models;

namespace MidReportTool.Api.Controllers
{
    //Implementing Clean Architecture pattern, so this class is used only as HTTP Controller, no bussiness logic is being placed around, only calling application layer for manipulating
    //and working with the data received in post methods. That is also valid for Get methods, from the controller, service classes are called for retrieving data and sending back to 
    //the user.
    [Route("api/v1/timeseries")]
    [ApiController]
    public class TimeSeriesDataController : ControllerBase
    {
        //Following Dependency Inversion Principle
        private readonly IWeatherDailyClimateService _serviceWeatherDailyClimate;

        private readonly ILogger<TimeSeriesDataController> _logger;

        public TimeSeriesDataController(ILogger<TimeSeriesDataController> logger, IWeatherDailyClimateService service)
        {
            _logger = logger;
            _serviceWeatherDailyClimate = service;
        }


        //Endpoint which to be responsible for uploading Weather Daily Climate
        //For v1 of the service, its available only a CSV file, for future is planned also json
        [HttpPost(Name = "upload/WeatherDailyClimate")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadWeatherDailyClimateChangesDTO request)
        {
            if (request.CsvFile == null || request.CsvFile.Length == 0)
            {
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "CSV file is missing!",
                    Error = "No file was uploaded!",
                    Status = "BadRequest"
                });
            }

            Result result = await _serviceWeatherDailyClimate.UploadAsync(request);
            if (result.Status.Equals(ApplicationStatus.Success))
            {
                return Ok(new ApiResponse
                {
                    Success = true,
                    Error = "",
                    Status = ApplicationStatus.Success.ToString(),
                    Message = ApiMessages.Success.Ok
                });
            }
            else if (result.Status.Equals(ApplicationStatus.Error) || result.Status.Equals(ApplicationStatus.ValidationError))
            {
                return BadRequest(new ApiResponse
                {
                    Success = true,
                    Error = result.ErrorMessage,
                    Status = ApplicationStatus.Error.ToString(),
                    Message = ApiMessages.Errors.UploadWeatherDailyClimateProblemDB
                });
            }
            else
            {
                return StatusCode(500, new ApiResponse
                {
                    Success = false,
                    Error = "Unhandled status",
                    Status = ApplicationStatus.Error.ToString(),
                    Message = ApiMessages.Errors.UploadWeatherDailyClimateProblemDB
                });
            }
        }

        //Standard endpoint which returns back all the data related to daily climate 
        //No filtering is being made, only pure data. Designed like that, because its expected to return consistent result which allowing caching 
        [HttpGet(Name = "Base view of all data which present in DB")]
        public async Task<IActionResult> GetAllRecords()
        {
            ResultT<List<WeatherDailyClimateDto>> task = await _serviceWeatherDailyClimate.GetAllRecordsAsync();
            if (task.Status.Equals(ApplicationStatus.Success))
            {
                return Ok(new ApiResponseData<List<WeatherDailyClimateDto>>
                {
                    Status = task.Status.ToString(),
                    Data = task.Data,
                });
            }

            return StatusCode(500, new ApiResponse
            {
                Success = false,
                Error = "Unhandled status",
                Status = ApplicationStatus.Error.ToString(),
                Message = ApiMessages.Errors.UploadWeatherDailyClimateProblemDB
            });
        }

        [HttpGet("search/{city}", Name = "Specific records from the DB by city name")]
        public async Task<IActionResult> GetSpecificRecord(string city)
        {
            ResultT<List<WeatherDailyClimateDto>> task =  await _serviceWeatherDailyClimate.GetSpecificRecordsByCityAsync(city);
            if (task.IsSuccess)
            {
                return Ok(new ApiResponseData<List<WeatherDailyClimateDto>>
                {
                    Status = task.Status.ToString(),
                    Data = task.Data,
                });
            }

            return BadRequest(new ApiResponse
            {
                Success = true,
                Error = "",
                Status = ApplicationStatus.Error.ToString(),
                Message = ApiMessages.Errors.UploadWeatherDailyClimateProblemDB
            });
        }

        [HttpPost("search",Name = "Version 1 of Filtering all data based on criterias")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetFilteredDTOsV1([FromBody] WeatherFilterRequest filter)
        {
            ResultT<List<WeatherDailyClimateDto>> resultT = await _serviceWeatherDailyClimate.GetFilteredRecordsAsyncV1(filter);

            if (resultT.IsSuccess) {
                return Ok(new ApiResponseData<List<WeatherDailyClimateDto>>
                {
                    Status = resultT.Status.ToString(),
                    Data = resultT.Data,
                });
            }

            return BadRequest(new ApiResponse
            {
                Success = true,
                Error = "",
                Status = ApplicationStatus.Error.ToString(),
                Message = ApiMessages.Errors.NotFound
            });
        }
    }
}
