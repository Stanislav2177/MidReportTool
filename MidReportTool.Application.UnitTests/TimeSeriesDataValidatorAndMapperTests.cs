
using FluentAssertions;
using MidReportTool.Api.Models;
using MidReportTool.ApplicationServices.Interfaces;
using MidReportTool.ApplicationServices.Services;
using MidReportTool.Domain;
using Moq;

namespace MidReportTool.Application.UnitTests
{
    public class TimeSeriesDataValidatorAndMapperTests
    {
        private readonly WeatherCsvService _service;

        public TimeSeriesDataValidatorAndMapperTests()
        {
            _service = new WeatherCsvService();
        }

        // ------------------------------------------------------------
        // IndexesOfColumns
        // ------------------------------------------------------------
        [Fact]
        public void CheckCorrectnesOfDeserializingWrongHeader()
        {
            string wrongHeader = "city, lat, lng, country, iso2, admin_name, capital, population, population_proper";
            List<string> columns = new List<string> { "l1", "l2", "l3", "l4", "l5"};
            char separator = ',';

            int[] ints = _service.IndexesOfColumns(columns, separator, wrongHeader);
            ints.Should().OnlyContain(x => x == -1);
        }

        [Fact]
        public void CheckCorrectnesOfDeserializingCorrectHeader()
        {
            string wrongHeader = "city, lat, lng, country, iso2, admin_name, capital, population, population_proper";
            List<string> columns = new List<string> { "city", "country", "admin_name", "capital", "population_proper" };
            char separator = ',';

            int[] ints = _service.IndexesOfColumns(columns, separator, wrongHeader);
            ints.Should().Equal(new int[] { 0, 3, 5, 6, 8 });
        }

        [Fact]
        public void CheckCorrectnesOfDeserializingAlmostCorrectHeader()
        {
            string wrongHeader = "city, lat, lng, country, iso2, admin_name, capital, population, population_proper";
            List<string> columns = new List<string> { "city", "l2", "admin_name", "capital", "population_proper" };
            char separator = ',';

            int[] ints = _service.IndexesOfColumns(columns, separator, wrongHeader);
            ints.Should().Equal(new int[] { 0, -1, 5, 6, 8 });
        }

        [Fact]
        public void IndexesOfColumns_ShouldReturnMinusOneForMissingColumns()
        {
            string header = "col1,col2,col3";
            List<string> columns = new() { "col1", "xxx", "col3" };

            var result = _service.IndexesOfColumns(columns, ',', header);

            result.Should().Equal(new[] { 0, -1, 2 });
        }

        [Fact]
        public void IndexesOfColumns_ShouldParseCorrectHeaderIndexes()
        {
            string header = "city,lat,lng,country";
            List<string> columns = new() { "city", "country" };

            var result = _service.IndexesOfColumns(columns, ',', header);

            result.Should().Equal(new[] { 0, 3 });
        }

        [Fact]
        public void ExtractColumns_ShouldReturnOnlyNonNullColumns()
        {
            var dto = new UploadWeatherDailyClimateChangesDTO
            {
                MeantempMapping = "MeanTemp",
                HumidityMapping = "Humidity",
                DateMapping = null,
                MeanpressureMapping = "",
                WindSpeedMapping = "Wind",
                RelatedCityMapping = null
            };

            var result = _service.ExtractColumns(dto);

            result.Should().BeEquivalentTo(new List<string>
            {
                "MeanTemp",
                "Humidity",
                "Wind"
            });
        }

        // ------------------------------------------------------------
        // DetectSeparator
        // ------------------------------------------------------------
        [Theory]
        [InlineData("a,b,c", ',', "comma")]
        [InlineData("a;b;c", ';', "semicolon")]
        [InlineData("a|b|c", '|', "pipe")]
        [InlineData("a\tb\tc", '\t', "tab")]
        public void DetectSeparator_ShouldReturnCorrectSeparator(string header, char expected, string reason)
        {
            var sep = _service.DetectSeparator(header);

            sep.Should().Be(expected);
        }

        // ------------------------------------------------------------
        // ValidateHeader
        // ------------------------------------------------------------

        [Fact]
        public void ValidateHeader_ShouldReturnErrorsForMissingColumns()
        {
            string header = "name,age,country";
            char sep = ',';
            List<string> expected = new() { "name", "country", "city" };

            var errors = _service.ValidateHeader(header, sep, expected);

            errors.Should().ContainSingle("Missing required column: city");
        }

        [Fact]
        public void ValidateHeader_ShouldReturnEmptyListWhenAllColumnsExist()
        {
            string header = "name,age,country";
            List<string> expected = new() { "name", "age" };

            var errors = _service.ValidateHeader(header, ',', expected);

            errors.Should().BeEmpty();
        }

        // ------------------------------------------------------------
        // ValidateIndexes
        // ------------------------------------------------------------

        [Fact]
        public void ValidateIndexes_ShouldReturnFalseIfAnyIndexIsMinusOne()
        {
            int[] indexes = { 0, 1, -1, 5 };

            var result = _service.ValidateIndexes(indexes);

            result.Should().BeFalse();
        }

        [Fact]
        public void ValidateIndexes_ShouldReturnTrueIfAllIndexesAreValid()
        {
            int[] indexes = { 0, 1, 2, 3 };

            var result = _service.ValidateIndexes(indexes);

            result.Should().BeTrue();
        }
    }
}
