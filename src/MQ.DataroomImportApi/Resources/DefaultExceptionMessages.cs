using MQ.DataroomImportApi.Exceptions;

namespace MQ.DataroomImportApi.Resources
{
    public class DefaultExceptionMessages
    {
        public static Dictionary<ErrorCodes, string> Messages = new()
        {
            { ErrorCodes.Unknown, "Unknown" },
            { ErrorCodes.FileIsTooBig, "File is too big" }
        };
    }
}
