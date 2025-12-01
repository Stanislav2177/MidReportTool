namespace MidReportTool.CustomException
{
    public class GetSpecificRecordInDatabaseException : Exception
    {

        public GetSpecificRecordInDatabaseException() { }
        public GetSpecificRecordInDatabaseException(string message) : base(message) { }
        public GetSpecificRecordInDatabaseException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
