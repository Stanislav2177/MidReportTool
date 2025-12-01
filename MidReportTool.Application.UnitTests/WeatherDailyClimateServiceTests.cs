using Moq;
using Xunit;
using FluentAssertions;
using System.IO;
using Microsoft.AspNetCore.Http;
using MidReportTool.ApplicationServices.Services;
using MidReportTool.ApplicationServices.Interfaces;
using MidReportTool.ApplicationServices.Models;
using MidReportTool.Domain;
using MidReportTool.Api.Responses;
using MidReportTool.CustomException;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using MidReportTool.Api.Models;

public class WeatherDailyClimateServiceTests
{
    private readonly Mock<IWeatherDailyClimateRepository> _repoMock;
    private readonly WeatherDailyClimateService _service;

    public WeatherDailyClimateServiceTests()
    {
        _repoMock = new Mock<IWeatherDailyClimateRepository>();
        _service = new WeatherDailyClimateService(_repoMock.Object);
    }

    private IFormFile CreateCsvFile(string csvContent)
    {
        var bytes = Encoding.UTF8.GetBytes(csvContent);
        var stream = new MemoryStream(bytes);
        return new FormFile(stream, 0, bytes.Length, "csvFile", "file.csv");
    }

    [Fact]
    public async Task UploadAsync_Should_Return_Success_When_CSV_Is_Valid()
    {
        string csv = "City,Date,Temp,Hum,Wind,Pressure\n" +
                     "sf,2025-05-06,10,50,5,1000";

        var formFile = CreateCsvFile(csv);
        var dto = new UploadWeatherDailyClimateChangesDTO { CsvFile = formFile };

        _repoMock.Setup(r => r.AddRangeAsync(It.IsAny<List<WeatherDailyClimate>>()))
                 .Returns(Task.CompletedTask);

        var result = await _service.UploadAsync(dto);
        result.Status.Should().Be(ApplicationStatus.Success);
        _repoMock.Verify(r => r.AddRangeAsync(It.IsAny<List<WeatherDailyClimate>>()), Times.Once);
    }

    [Fact]
    public async Task UploadAsync_Should_Return_ValidationError_When_CSV_Is_Invalid()
    {
        string csv = "Date,Temp,Hum,Wind,Pressure\n" +
                     "2025-11-21,10,abc,5,1000";

        var formFile = CreateCsvFile(csv);
        var dto = new UploadWeatherDailyClimateChangesDTO { CsvFile = formFile };

        var result = await _service.UploadAsync(dto);

        result.Status.Should().Be(ApplicationStatus.ValidationError);
        result.ErrorMessage.Should().Contain("Problem on line");
        _repoMock.Verify(r => r.AddRangeAsync(It.IsAny<List<WeatherDailyClimate>>()), Times.Never);
    }

    [Fact]
    public async Task UploadAsync_Should_Return_Error_When_Repository_Throws()
    {
        string csv = "Date,Temp,Hum,Wind,Pressure\n" +
                     "1999-01-11,10,50,5,1000";

        var formFile = CreateCsvFile(csv);
        var dto = new UploadWeatherDailyClimateChangesDTO { CsvFile = formFile };

        _repoMock
            .Setup(r => r.AddRangeAsync(It.IsAny<List<WeatherDailyClimate>>()))
            .ThrowsAsync(new AddRecordsInDatabaseException("DB error"));

        var result = await _service.UploadAsync(dto);

        result.Status.Should().Be(ApplicationStatus.Error);
        result.ErrorMessage.Should().Be("DB error");
    }

    [Fact]
    public async Task GetAllRecordsAsync_Should_Return_Records()
    {
        var entity = new WeatherDailyClimate("sofia",
            DateTime.Now, 10, 50, 5, 1000);

        _repoMock.Setup(r => r.GetAllAsync())
                 .ReturnsAsync(new List<WeatherDailyClimate> { entity });

        var result = await _service.GetAllRecordsAsync();

        result.Status.Should().Be(ApplicationStatus.Success);
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetSpecificRecordAsync_Should_Return_Record_When_Found()
    {
        var entity = new WeatherDailyClimate("sofia",DateTime.Now, 10, 50, 5, 1000);

        _repoMock.Setup(r => r.GetSpecificAsync(entity.City))
                 .ReturnsAsync(new List<WeatherDailyClimate> { entity });

        var result = await _service.GetSpecificRecordsByCityAsync(entity.City);

        result.Status.Should().Be(ApplicationStatus.Success);
        result.Data.Should().NotBeNull();
    }

    [Fact]
    public async Task GetSpecificRecordAsync_Should_Return_NotFound_When_Entity_Is_Null()
    {
        _repoMock.Setup(r => r.GetSpecificAsync("sofia"))
         .ReturnsAsync((List<WeatherDailyClimate>) null);

        var result = await _service.GetSpecificRecordsByCityAsync("sofia");

        result.Status.Should().Be(ApplicationStatus.NotFound);
    }

    [Fact]
    public async Task GetFilteredRecordsAsyncV1_Should_Return_Data()
    {
        var entity = new WeatherDailyClimate("sofia", DateTime.Now, 10, 50, 5, 1000);

        _repoMock.Setup(r => r.GetFilteredRecordsAsyncV1(It.IsAny<WeatherFilterCriteria>()))
                 .ReturnsAsync(new List<WeatherDailyClimate> { entity });

        var request = new WeatherFilterRequest { MeanTempMin = 0 };

        var result = await _service.GetFilteredRecordsAsyncV1(request);

        result.Status.Should().Be(ApplicationStatus.Success);
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetFilteredRecordsAsyncV1_Should_Return_NotFound_When_No_Data()
    {
        _repoMock.Setup(r => r.GetFilteredRecordsAsyncV1(It.IsAny<WeatherFilterCriteria>()))
                 .ReturnsAsync(new List<WeatherDailyClimate>());

        var request = new WeatherFilterRequest();

        var result = await _service.GetFilteredRecordsAsyncV1(request);

        result.Status.Should().Be(ApplicationStatus.NotFound);
    }


}
