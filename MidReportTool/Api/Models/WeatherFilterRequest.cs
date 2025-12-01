namespace MidReportTool.Api.Models
{
    public class WeatherFilterRequest
    {
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public double? MeanTempMin { get; set; }
        public double? MeanTempMax { get; set; }
        public double? HumidityMin { get; set; }
        public double? HumidityMax { get; set; }
        public double? WindSpeedMin { get; set; }
        public double? WindSpeedMax { get; set; }
        public double? MeanPressureMin { get; set; }
        public double? MeanPressureMax { get; set; }
    }

}
