namespace MidReportTool.Api.Responses
{
    public static class ApiMessages
    {
        public static class Success
        {
            public const string Ok = "Operation completed successfully.";
        }

        public static class Errors
        {
            public const string UploadWeatherDailyClimateProblemDB = "Problem appeared when saving to Database";
            public const string UploadWeatherDailyClimateProblemMapping = "Problem appeared when mapping to ";
            public const string NotFound = "No records found";
        }

        public static class Process
        {
            public const string GeneratingCache = "Process of generating cache is started, please access the data after couple of minutes";
        }
    }
}
