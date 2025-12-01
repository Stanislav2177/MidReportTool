namespace MidReportTool.ApplicationServices.Models
{
    public class ResultT<T>
    {
        public ApplicationStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public T? Data { get; set; }

        public bool IsSuccess => Status == ApplicationStatus.Success;

        public static ResultT<T> Success(T data) => new ResultT<T>
        {
            Status = ApplicationStatus.Success,
            Data = data
        };

        public static ResultT<T> Failure(ApplicationStatus status, string? message = null)
            => new ResultT<T>
            {
                Status = status,
                ErrorMessage = message
            };
    }
}
