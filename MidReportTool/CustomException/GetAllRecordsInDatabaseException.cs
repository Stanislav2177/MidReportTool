namespace MidReportTool.CustomException
{
    public class GetAllRecordsInDatabaseException : Exception
    {
        public GetAllRecordsInDatabaseException() { }
        public GetAllRecordsInDatabaseException(string message) : base(message) { }
        public GetAllRecordsInDatabaseException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
