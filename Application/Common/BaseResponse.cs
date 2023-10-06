namespace Application.Common
{
    public class BaseResponse<TResponse>
    {
        public bool Error { get; set; } = false;
        public string Message { get; set; } = null!;
        public TResponse? Result { get; set; }
        public Exception Exception { get; set; } = null!;

        public BaseResponse()
        {

        }

        public BaseResponse(bool error, string message, Exception exception)
        {
            Error = error;
            Message = message;
            Exception = exception;
        }
    }
}
