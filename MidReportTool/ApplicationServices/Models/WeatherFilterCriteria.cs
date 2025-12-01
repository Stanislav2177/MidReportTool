namespace MidReportTool.ApplicationServices.Models
{
    public class WeatherFilterCriteria
    {
        public int Id { get; set; } = -1;
        public DateTime? DateFrom { get; set; }
        public DateTime? DateTo { get; set; }
        public double? MeanTempMin { get; set; }
        public double? MeanTempMax { get; set; }
        public double? HumidityMin { get; set; }
        public double? HumidityMax { get; set; }
    }
}
