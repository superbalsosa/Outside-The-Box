namespace Utilities.Error
{
    public class ErrorUtility
    {
        public ErrorType ErrorType { get; }
        public bool IsSuccess { get; }
        public string Message { get; }

        public ErrorUtility(ErrorType errorType, bool isSuccess, string message)
        {
            ErrorType = errorType;
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ErrorUtility Success(ErrorType errorType, string message = "OK") => new ErrorUtility(errorType, true, message);
        public static ErrorUtility Error(ErrorType errorType, string message) => new ErrorUtility(errorType, true, message);
    }
}
