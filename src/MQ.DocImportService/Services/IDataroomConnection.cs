using MQ.DocImportService.Models.Import;
using MQ.Domain.Queue.Models;

namespace MQ.DocImportService.Services
{
    public interface IDocStorageConnection : IDisposable
    {
        /// <summary>
        /// Создание структуры папок
        /// </summary>
        /// <param name="list">Список папок, которые нужно создать</param>
        /// <returns>конечная папка</returns>
        Task<object?> CreateFolders(List<BasicFolder> foldersList);

        Task<ImportedDocInfo> UploadDocument(object folder, DocForDataroomQueueItem docInfo);
    }
}
