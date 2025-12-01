namespace MidReportTool.CustomException
{
    public class AddRecordsInDatabaseException : Exception
    {
        public AddRecordsInDatabaseException() { }
        public AddRecordsInDatabaseException(string message) : base(message) { }
        public AddRecordsInDatabaseException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
