namespace MidReportTool.Api.Responses
{
    public class ApiResponseData<T>
    {
        public string Status { get; set; } = string.Empty;
        public T Data { get; set; }
    }
}
