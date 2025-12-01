namespace MidReportTool.Api.Models
{
    public class UploadWeatherDailyClimateChangesDTO
    {
        public string? Name { get; set; }      
        public string? Description { get; set; }
        public IFormFile CsvFile { get; set; }
        public string DateTimeFormat {  get; set; }
        public string DateMapping { get; set; }
        public string MeantempMapping { get; set; }
        public string HumidityMapping { get; set; }
        public string WindSpeedMapping { get; set; }
        public string MeanpressureMapping { get; set; }
        public string RelatedCityMapping { get; set; }

    }
}
