using MQ.DataroomImportApi.Resources;
using System.Text.Json;

namespace MQ.DataroomImportApi.Exceptions
{
    public class ApiException : Exception
    {
        public int HttpCode { get; init; }
        public ErrorCodes ErrorCode { get; init; }
        public string? Error { get; init; }

        public ApiException(ErrorCodes errorCode, int? httpCode = null, string? error = null)
        {
            HttpCode = httpCode ?? 500;
            (ErrorCode, Error) = (errorCode, error);

            if (error is null)
                Error = GetErrorMessage(errorCode);
        }

        private string? GetErrorMessage(ErrorCodes errorCode)
        {
            if (DefaultExceptionMessages.Messages.ContainsKey(errorCode))
                return DefaultExceptionMessages.Messages[errorCode];
            else
                return errorCode.ToString();
        }

        public string GetJsonResponce()
        {
            return JsonSerializer.Serialize(this);
        }
    }
    public enum ErrorCodes
    {
        Unknown,
        FileIsTooBig,
        ArgumentIsNull
    }
}
