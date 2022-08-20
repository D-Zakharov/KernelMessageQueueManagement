using System.Text.Json.Serialization;

namespace MQ.DataroomImportApi.Exceptions
{
    /// <summary>
    /// Универсальный ответ об ошибке
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// код ошибки
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public ErrorCodes? ErrorCode { get; set; }

        /// <summary>
        /// описание ошибки
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? Error { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? ExceptionStack { get; set; }
    }
}
