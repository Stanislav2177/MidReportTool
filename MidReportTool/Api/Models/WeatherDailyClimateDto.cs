namespace MidReportTool.Api.Models
{
    public class WeatherDailyClimateDto
    {
        
        public int Id { get; set; }
        public string City { get; set; }
        public DateTime Date { get; set; }
        public double Meantemp { get; set; }
        public double Humidity { get; set; }
        public double WindSpeed { get; set; }
        public double MeanPressure { get; set; }

        public WeatherDailyClimateDto(string city, int id, DateTime date, double MeanTemp, double Humidity, double WindSpeed, double MeanPressure) {
            this.City = city;
            this.Id = id;
            this.Date = date;   
            this.Meantemp = MeanTemp;
            this.Humidity = Humidity;
            this.WindSpeed = WindSpeed;
            this.MeanPressure = MeanPressure;
        }
    }
}
