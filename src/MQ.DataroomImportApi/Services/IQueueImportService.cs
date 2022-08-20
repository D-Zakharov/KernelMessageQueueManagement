using MQ.DataroomImportApi.Models;

namespace MQ.DataroomImportApi.Services;

public interface IQueueImportService
{
    Task<IResult> SendDocumentToQueue(ImportRequestModel docData);
}