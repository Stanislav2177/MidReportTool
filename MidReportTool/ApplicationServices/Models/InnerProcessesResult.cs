namespace MidReportTool.ApplicationServices.Models
{
    //Internal class which is being used only in the current assembly, the idea is to handle results like error or some statuses from the processes which are being triggered
    //and that results to be provided on the upper level
    internal class InnerProcessesResult
    {
        private string _message = string.Empty;

        internal string Message
        {
            get => _message;
            set => _message +=  Environment.NewLine + value;
        }

        internal bool Successes { get; set; } = true;
        internal List<string> Errors { get; set; } = new();


    }
}
