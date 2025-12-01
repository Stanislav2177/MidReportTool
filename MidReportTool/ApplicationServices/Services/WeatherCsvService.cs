using MidReportTool.Api.Models;

namespace MidReportTool.ApplicationServices.Services
{
    public class WeatherCsvService
    {
        public readonly string[] SupportedDateFormats = new[]
        {   
            "yyyy-MM-dd",
            "dd-MM-yyyy",
            "d-M-yyyy",
            "MM-dd-yyyy",
            "M-d-yyyy",
            "yyyy/MM/dd",
            "dd/MM/yyyy",
            "d/M/yyyy",
            "MM/dd/yyyy",
            "M/d/yyyy",
            "dd.MM.yyyy",
            "d.M.yyyy",
            "yyyy.MM.dd"
        };

        public List<string> ExtractColumns(UploadWeatherDailyClimateChangesDTO dto)
        {
            var result = new List<string>();

            if (!string.IsNullOrWhiteSpace(dto.RelatedCityMapping))
                result.Add(dto.RelatedCityMapping);

            if (!string.IsNullOrWhiteSpace(dto.DateMapping))
                result.Add(dto.DateMapping);
            if (!string.IsNullOrWhiteSpace(dto.MeantempMapping))
                result.Add(dto.MeantempMapping);

            if (!string.IsNullOrWhiteSpace(dto.HumidityMapping))
                result.Add(dto.HumidityMapping);

            if (!string.IsNullOrWhiteSpace(dto.WindSpeedMapping))
                result.Add(dto.WindSpeedMapping);

            if (!string.IsNullOrWhiteSpace(dto.MeanpressureMapping))
                result.Add(dto.MeanpressureMapping);
            return result;
        }

        public int[] IndexesOfColumns(List<string> columns, char separator, string header)
        {
            int[] indexes = new int[columns.Count];
            List<string> heads = header.Split(separator).Select(h => h.Trim()).ToList();

            for (int i = 0; i < columns.Count; i++)
            {
                int idx = heads.IndexOf(columns[i]);
                indexes[i] = idx >= 0 ? idx : -1;
            }

            return indexes;
        }

        public char DetectSeparator(string header)
        {
            char[] possibleSeparators = { ',', ';', '|', '\t' };
            var value = possibleSeparators
                .Select(sep => new { Sep = sep, Count = header.Count(c => c == sep) })
                .OrderByDescending(x => x.Count)
                .First();

            if (value != null)
            {
                return value.Sep;
            }

            return 'x';
        }

        public List<string> ValidateHeader(string header, char separator, List<string> expectedColumns)
        {
            var errors = new List<string>();
            var headerColumns = header
                .Split(separator)
                .Select(h => h.Trim())
                .ToList();

            foreach (var expected in expectedColumns)
            {
                if (!headerColumns.Contains(expected))
                {
                    errors.Add($"Missing required column: {expected}");
                }
            }

            return errors;
        }

        public bool ValidateIndexes(int[] arr)
        {
            return !arr.Contains(-1);
        }

        public bool IsValidDateTimeFormat(string format)
        {
            return SupportedDateFormats.Contains(format);
        }
    }
}
