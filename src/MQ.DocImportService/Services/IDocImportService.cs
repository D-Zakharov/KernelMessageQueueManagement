using MQ.DocImportService.Models.Import;
using MQ.Domain.Queue.Models;

namespace MQ.DocImportService.Services;

public interface IDocImportService : IDisposable
{
    Task ClearTempFiles(DocForDataroomQueueItem message);
    Task<ImportedDocInfo> ImportDocument(DocForDataroomQueueItem message);
    Task SaveImportedDocumentInfo(DocForDataroomQueueItem message, ImportedDocInfo importResult);
}
