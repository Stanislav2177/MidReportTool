using System.Globalization;
using System.Text;
using MidReportTool.Api.Models;
using MidReportTool.ApplicationServices.Interfaces;
using MidReportTool.ApplicationServices.Models;
using MidReportTool.CustomException;
using MidReportTool.Domain;

namespace MidReportTool.ApplicationServices.Services
{
    public class WeatherDailyClimateService : IWeatherDailyClimateService
    {
        private readonly IWeatherDailyClimateRepository _repository;
        private WeatherCsvService _serviceCSV;

        public WeatherDailyClimateService(IWeatherDailyClimateRepository repository)
        {
            _repository = repository;
            _serviceCSV = new WeatherCsvService();
        }

        public async Task<Result> UploadAsync(UploadWeatherDailyClimateChangesDTO dto)
        {
            Result result = new Result();
            try
            {
                //Mapping the CSV to usable data structure of objects
                (InnerProcessesResult, List<WeatherDailyClimate>) value = MapCSVToObject(dto);
                if (!value.Item1.Successes)
                {
                    result.Status = ApplicationStatus.ValidationError;
                    result.ErrorMessage = GetAllErrorsFromList(value.Item1.Errors);
                    return result;
                }

                await _repository.AddRangeAsync(value.Item2);

                //If no error appear, we consider the executing of the query for successful, so the success is set to true
                result.Status = ApplicationStatus.Success;
            }
            //Catches custom exception, by this approach we are trying to follow what is going on in the infrastructure layer
            catch (AddRecordsInDatabaseException ex)
            {
                result.Status = ApplicationStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<ResultT<List<WeatherDailyClimateDto>>> GetAllRecordsAsync()
        {
            ResultT<List<WeatherDailyClimateDto>> result = new ResultT<List<WeatherDailyClimateDto>>();
            try
            {
                var records = await _repository.GetAllAsync();

                //Converting Entity object to DTO, which to be send to API Layer
                //Following clean architeture pattern, so its not allowed to expose domain layer entities to API Layer
                result.Data = records
                    .Select(x => EntityToDTO(x))
                    .ToList();

                result.Status = ApplicationStatus.Success;
            }
            catch (GetAllRecordsInDatabaseException ex)
            {
                result.Status = ApplicationStatus.Error;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        public async Task<ResultT<List<WeatherDailyClimateDto>>> GetSpecificRecordsByCityAsync(string city)
        {
            ResultT<List<WeatherDailyClimateDto>> result = new ResultT<List<WeatherDailyClimateDto>>();

            try
            {
                List<WeatherDailyClimate> weatherDailyClimate = await _repository.GetSpecificAsync(city);

                if (weatherDailyClimate == null)
                {
                    result.Status = ApplicationStatus.NotFound;
                    return result;
                }

                result.Status = ApplicationStatus.Success;
                result.Data = new List<WeatherDailyClimateDto>();
                weatherDailyClimate.ForEach(x => result.Data.Add(EntityToDTO(x)));

            }
            catch (GetSpecificRecordInDatabaseException ex)
            {
                result.ErrorMessage = ex.Message;
                result.Status = ApplicationStatus.Error;
            }

            return result;
        }

        public async Task<ResultT<List<WeatherDailyClimateDto>>> GetFilteredRecordsAsyncV1(WeatherFilterRequest request)
        {
            ResultT<List<WeatherDailyClimateDto>> result = new ResultT<List<WeatherDailyClimateDto>>();
            try
            {
                WeatherFilterCriteria criteria = new WeatherFilterCriteria
                {
                    DateFrom = request.DateFrom,
                    DateTo = request.DateTo,
                    HumidityMax = request.HumidityMax,
                    HumidityMin = request.HumidityMin,
                    MeanTempMax = request.MeanTempMax,
                    MeanTempMin = request.MeanTempMin
                };

                List<WeatherDailyClimate> task = await _repository.GetFilteredRecordsAsyncV1(criteria);

                if (task.Count == 0)
                {
                    result.Status = ApplicationStatus.NotFound;
                    return result;
                }

                result.Status = ApplicationStatus.Success;
                result.Data = task
                    .Select(x => EntityToDTO(x))
                    .ToList();
            }
            catch (GetSpecificRecordInDatabaseException ex)
            {
                result.ErrorMessage = ex.Message;
                result.Status = ApplicationStatus.Error;
            }

            return result;
        }
        private WeatherDailyClimateDto EntityToDTO(WeatherDailyClimate entity)
        {
            return new WeatherDailyClimateDto(entity.City, entity.Id, entity.Date, entity.Meantemp, entity.Humidity, entity.WindSpeed, entity.MeanPressure);
        }

        private (InnerProcessesResult, List<WeatherDailyClimate>) MapCSVToObject(UploadWeatherDailyClimateChangesDTO dto)
        {
            InnerProcessesResult inner = new InnerProcessesResult();
            List<WeatherDailyClimate> list = new List<WeatherDailyClimate>();

            inner.Message = "Result from: Map CSV File To Object";

            try
            {
                using (var reader = new StreamReader(dto.CsvFile.OpenReadStream()))
                {
                    string currentLine;

                    //Reads the header part before the loop
                    string? header = reader.ReadLine();

                    //Its not expected this to be triggered, but for symetry of checking edge cases
                    if (header == null)
                    {
                        inner.Errors.Add("No header found");
                        inner.Successes = false;
                        return (inner, list);
                    }

                    if (!_serviceCSV.IsValidDateTimeFormat(dto.DateTimeFormat))
                    {
                        SetInnerProcessProperties(inner, "No valid datetime format", new List<string>(), false);
                        return (inner, list);
                    }

                    //Level:1 Detecting what separator is used 
                    char separator = _serviceCSV.DetectSeparator(header);
                    if(separator == 'x')
                    {
                        SetInnerProcessProperties(inner, "No valid separator was found", new List<string>(), false);
                        return (inner, list);
                    }

                    //As request body in API is required, its not expected to have missing columns
                    List<string> listColumns = _serviceCSV.ExtractColumns(dto);

                    List<string> listErrors = _serviceCSV.ValidateHeader(header, separator, listColumns);

                    //If errors appeared during validation, its return back to main method and then to API
                    if (listErrors.Count > 0)
                    {
                        SetInnerProcessProperties(inner, "Validation failed in header part", listErrors, false);
                        return (inner, list);
                    }

                    int[] indexes = _serviceCSV.IndexesOfColumns(listColumns, separator, header);
                    if (_serviceCSV.ValidateIndexes(indexes))
                    {
                        int counter = 1;
                        while ((currentLine = reader.ReadLine()) != null)
                        {
                            var headerCols = currentLine.Split(separator);

                            try
                            {
                                var record = new WeatherDailyClimate(
                                    headerCols[indexes[0]],
                                DateTime.ParseExact(headerCols[indexes[1]], _serviceCSV.SupportedDateFormats, CultureInfo.InvariantCulture, DateTimeStyles.None),
                                    double.Parse(headerCols[indexes[2]], CultureInfo.InvariantCulture),
                                    double.Parse(headerCols[indexes[3]], CultureInfo.InvariantCulture),
                                    double.Parse(headerCols[indexes[4]], CultureInfo.InvariantCulture),
                                    double.Parse(headerCols[indexes[5]], CultureInfo.InvariantCulture)
                                );

                                list.Add(record);
                            }
                            catch (Exception ex)
                            {
                                inner.Errors.Add($"Problem on line {counter}: {ex.Message}");
                                inner.Successes = false;

                                //This also can be made to traverse all lines and collect all errors by commenting the return statement. But for now, it will return back 
                                //response with the specific line and line problem.
                                return (inner, list);
                            }
                        }
                    }
                    else
                    {
                        SetInnerProcessProperties(inner, "Validation failed in receiving indexes", listErrors, false);
                        return (inner, list);
                    }
                }
            }
            catch (Exception ex)
            {
                inner.Message = $"Outer Error appeared in mapping CSV to Object, with message: {ex.Message}";
            }

            if (inner.Successes)
            {
                inner.Message = "No Errors";
            }

            return (inner, list);
        }

        private void SetInnerProcessProperties(InnerProcessesResult inner, string msg, List<string> errors, bool isSuccess)
        {
            inner.Message = msg;
            inner.Errors = errors;
            inner.Successes = isSuccess;
        }
        private string GetAllErrorsFromList(List<string> list)
        {
            StringBuilder sb = new StringBuilder();
            list.ForEach(error => { sb.AppendLine(error); });
            return sb.ToString();
        }
    }
}
