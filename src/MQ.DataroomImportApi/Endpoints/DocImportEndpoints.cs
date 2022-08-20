using MQ.DataroomImportApi.Models;
using MQ.DataroomImportApi.Services;

namespace MQ.DataroomImportApi.Endpoints
{
    public static class DocImportEndpoints
    {
        private static readonly Dictionary<string, EventId> ApiEvents = new()
        {
            { nameof(ImportDocument), new EventId(0, nameof(ImportDocument)) }
        };

        public static void MapDocImportEndpoints(this WebApplication app)
        {
            app.MapPut("/api/v1/documents/Import", ImportDocument).Accepts<ImportRequestModel>("multipart/form-data");
        }

        private static async Task<IResult> ImportDocument(HttpContext ctx, IQueueImportService dataroomImportService)
        {
            ImportRequestModel docData = new (ctx); //min apis на данный момент не поддерживают файлы в form-data

            var res = await dataroomImportService.SendDocumentToQueue(docData);
            
            return Results.Ok(res);
        }
    }
}
