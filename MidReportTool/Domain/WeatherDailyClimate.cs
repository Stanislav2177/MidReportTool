using MidReportTool.CustomException;

namespace MidReportTool.Domain
{
    public class WeatherDailyClimate
    {
        //Property which is refer for DB, its used as primary key for searching and etc.
        public int Id { get; private set; }
        public string City { get; private set; }
        public DateTime Date { get; private set; }
        public double Meantemp { get; private set; }
        public double Humidity { get; private set; }
        public double WindSpeed { get; private set; }
        public double MeanPressure { get; private set; }

        //Empty constructor required by EF Core, otherwise error appear: 'Note that only mapped properties can be bound to constructor parameters. Navigations to related entities, including references to owned types, cannot be bound.''
        private WeatherDailyClimate() { }


        //As i read, this is good practice to have a centralized place where the necessery bussiness rules to be applied,
        //This is a Entity, place in the domain, which entity in the workflow will be send to the Repository
        public WeatherDailyClimate(string city, DateTime Date, double Meantemp, double Humidity, double WindSpeed, double MeanPressure)
        {
            if (String.IsNullOrEmpty(city))
                throw new DomainException("City name cannot be empty.");

            if (Date == default)
                throw new DomainException("Date cannot be empty.");

            if (Meantemp < -50 || Meantemp > 60)
                throw new DomainException($"Meantemp {Meantemp} is out of range (-50 to 60).");

            if (Humidity < 0 || Humidity > 1500)
                throw new DomainException($"Humidity {Humidity} must be between 0 and 100.");

            if (WindSpeed < 0)
                throw new DomainException($"Wind speed {WindSpeed} cannot be negative.");

            if (MeanPressure <= 0)
                throw new DomainException($"Mean pressure {MeanPressure} must be positive.");

            this.City = city;
            this.Date = Date;
            this.Meantemp = Meantemp;
            this.Humidity = Humidity;
            this.WindSpeed = WindSpeed;
            this.MeanPressure = MeanPressure;
        }

        // Internal helper for constructor and update
        public void SetValues(string city, DateTime date, double meantemp, double humidity, double windSpeed, double meanPressure)
        {
            if (meantemp < -50 || meantemp > 60)
                throw new DomainException($"Invalid temperature: {meantemp}");
            if (humidity < 0 || humidity > 100)
                throw new DomainException($"Invalid humidity: {humidity}");
            if (windSpeed < 0)
                throw new DomainException($"Invalid wind speed: {windSpeed}");
            if (meanPressure <= 0)
                throw new DomainException($"Invalid pressure: {meanPressure}");

            Date = date;
            Meantemp = meantemp;
            Humidity = humidity;
            WindSpeed = windSpeed;
            MeanPressure = meanPressure;
        }

        private bool IsValidTemperature(double temperature)
        {
            return temperature >= -50 && temperature <= 60;
        }
    }
}
