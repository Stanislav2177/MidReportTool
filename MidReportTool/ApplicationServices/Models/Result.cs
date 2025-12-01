namespace MidReportTool.ApplicationServices.Models
{
    public class Result
    {
        public ApplicationStatus Status { get; set; }
        public string? ErrorMessage { get; set; }

        public bool IsSuccess => Status == ApplicationStatus.Success;

        public static Result Success() => new Result { Status = ApplicationStatus.Success };
        public static Result Failure(ApplicationStatus status, string? message = null)
            => new Result { Status = status, ErrorMessage = message };
    }

}
