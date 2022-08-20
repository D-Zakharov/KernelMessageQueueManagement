using System.Text.Json;
using Microsoft.AspNetCore.Diagnostics;
using static System.Net.Mime.MediaTypeNames;

namespace MQ.DataroomImportApi.Exceptions;

public static class ExceptionExtensionMethods
{
    internal static void RegisterExceptionHandler(this WebApplication app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                context.Response.ContentType = Application.Json;

                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

                if (exceptionHandlerPathFeature?.Error is ApiException exception)
                {
                    context.Response.StatusCode = exception.HttpCode;
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                }

                await context.Response.WriteAsync(CreateJsonErrorResponce((ILoggerFactory)app.Services.GetRequiredService(typeof(ILoggerFactory)),
                    exceptionHandlerPathFeature?.Error));
            });
        });
    }

    internal static string CreateJsonErrorResponce(ILoggerFactory loggerFactory, Exception? e)
    {
        return JsonSerializer.Serialize(CreateErrorResponce(loggerFactory, e));
    }

    private static ApiError CreateErrorResponce(ILoggerFactory loggerFactory, Exception? e)
    {
        var logger = loggerFactory.CreateLogger("common");

        ApiError res = new();
#if DEBUG
        if (e != null)
        {
            res.ExceptionStack = e.StackTrace;
        }
#endif
        if (e is ApiException apiException)
        {
            logger.Log(LogLevel.Error, exception: apiException, message: $"ErrorCode: {apiException.ErrorCode}");
            res.ErrorCode = apiException.ErrorCode;
            res.Error = apiException.Error;
        }
        else if (e is Exception exception)
        {
            logger.Log(LogLevel.Error, exception, exception.Message);
            res.ErrorCode = ErrorCodes.Unknown;
            res.Error = exception.GetBaseException().Message;
        }
        else
        {
            res.ErrorCode = ErrorCodes.Unknown;
            res.Error = "Unknown exception";
        }

        return res;
    }
}