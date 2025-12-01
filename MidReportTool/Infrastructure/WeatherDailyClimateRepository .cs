using Microsoft.EntityFrameworkCore;
using MidReportTool.ApplicationServices.Interfaces;
using MidReportTool.ApplicationServices.Models;
using MidReportTool.CustomException;
using MidReportTool.Domain;

namespace MidReportTool.Infrastructure
{
    public class WeatherDailyClimateRepository : IWeatherDailyClimateRepository
    {
        private readonly AppDbContext _context;

        public WeatherDailyClimateRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WeatherDailyClimate entity)
        {
            _context.WeatherDailyClimates.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<WeatherDailyClimate> entities)
        {
            try
            {
                //No hashing is added, because this will make the process slower. Currently we got like 5-6 columns and to update the whole row is faster
                //than checking row MD5 and the calculating the matched entity MDF and comparing them.
                foreach (var entity in entities)
                {
                    var existing = await _context.WeatherDailyClimates
                                                 .FirstOrDefaultAsync(x => x.Date == entity.Date && x.City == entity.City);
                    if (existing != null)
                    {
                        existing.SetValues(entity.City, entity.Date, entity.Meantemp, entity.Humidity, entity.WindSpeed, entity.MeanPressure);
                    }
                    else
                    {
                        _context.WeatherDailyClimates.Add(entity);
                    }
                }

                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new AddRecordsInDatabaseException("Failed to save WeatherDailyClimate records.", ex);
            }
        }

        public async Task<IEnumerable<WeatherDailyClimate>> GetAllAsync()
        {
            try
            {
                List<WeatherDailyClimate> weatherDailyClimates = await _context.WeatherDailyClimates.ToListAsync();
                return weatherDailyClimates;
            }
            catch (Exception ex)
            {
                throw new GetAllRecordsInDatabaseException("Failed to load all records from Database: ", ex);
            }
        }

        public async Task<List<WeatherDailyClimate>> GetFilteredRecordsAsyncV1(WeatherFilterCriteria filter)
        {
            try
            {
                var query = _context.WeatherDailyClimates.AsQueryable();

                if (filter.DateFrom.HasValue)
                    query = query.Where(w => w.Date >= filter.DateFrom.Value);

                if (filter.DateTo.HasValue)
                    query = query.Where(w => w.Date <= filter.DateTo.Value);

                if (filter.MeanTempMin.HasValue)
                    query = query.Where(w => w.Meantemp >= filter.MeanTempMin.Value);

                if (filter.MeanTempMax.HasValue)
                    query = query.Where(w => w.Meantemp <= filter.MeanTempMax.Value);

                if (filter.HumidityMin.HasValue)
                    query = query.Where(w => w.Humidity >= filter.HumidityMin.Value);

                if (filter.HumidityMax.HasValue)
                    query = query.Where(w => w.Humidity <= filter.HumidityMax.Value);

                return await query.ToListAsync();
            }
            catch (Exception ex) 
            { 
                throw new GetSpecificRecordInDatabaseException("Failed to get specific records from DB:", ex);
            }
        }

        public async Task<List<WeatherDailyClimate>> GetSpecific(string city)
        {
            try
            {
                var result = await _context.WeatherDailyClimates
                     .Where(x => x.City == city)
                     .ToListAsync();

                return result;
            }
            catch(Exception ex)
            {
                throw new GetSpecificRecordInDatabaseException("Failed to get specific record from DB:", ex);
            }
        }

        public async Task<List<WeatherDailyClimate>> GetSpecificAsync(string city)
        {
            try
            {
                var result = await _context.WeatherDailyClimates
                     .Where(x => x.City == city)
                     .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                throw new GetSpecificRecordInDatabaseException("Failed to get specific record from DB:", ex);
            }
        }
    }
}
