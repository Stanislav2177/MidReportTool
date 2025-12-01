using System.Net.NetworkInformation;

namespace MidReportTool.Api.Responses
{
    public class ApiStatuses
    {
        public readonly static Status OK = new(200, "OK");
        public readonly static Status CREATED = new(201, "Created");
        public readonly static Status NOT_FOUND = new(404, "Not Found");
        public readonly static Status INTERNAL_SERVER_ERROR = new(500, "Internal Server Error");

        public class Status
        {
            public int Code { get; set; }
            public string StatusMsg { get; set; }
            public Status(int code, string msg)
            {
                Code = code;
                StatusMsg = msg;
            }
        }
    }
}
