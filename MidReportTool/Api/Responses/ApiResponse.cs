using Microsoft.AspNetCore.Mvc;

namespace MidReportTool.Api.Responses
{
    public class ApiResponse 
    {
        public bool Success { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public string Error { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
