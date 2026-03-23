using UnityEngine;

namespace Utilities.Error
{
    public class ErrorUtility
    {
        public ErrorType ErrorType { get; }
        public bool IsSuccess { get; }
        public string Message { get; }
        public string StackTrace { get; }

        public ErrorUtility(ErrorType errorType, bool isSuccess, string message, string stackTrace = "")
        {
            ErrorType = errorType;
            IsSuccess = isSuccess;
            Message = message;
            StackTrace = stackTrace;
        }

        public static ErrorUtility Success(ErrorType errorType, string message = "OK") => new ErrorUtility(errorType, true, message);
        public static ErrorUtility Error(ErrorType errorType, string message, string stackTrace = "") => new ErrorUtility(errorType, true, message, stackTrace);
    }
}
