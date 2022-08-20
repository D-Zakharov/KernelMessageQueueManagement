using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQ.DocImportService.Models.Import;
using MQ.DocImportService.Services;
using MQ.Domain.Queue.Models;

namespace MQ.Tests.Unit.Mock.Service
{
    internal class MockDocStorageConnection : IDocStorageConnection
    {
        public async Task<object?> CreateFolders(List<BasicFolder> folders)
        {
            var res = new StringBuilder();

            await Task.Run(() =>
            {
                foreach (var folder in folders)
                {
                    res.Append(folder.Name).Append('/');
                };
            });
            return res;
        }

        public void Dispose()
        {
        }

        public async Task<ImportedDocInfo> UploadDocument(object folder, DocForDataroomQueueItem docInfo)
        {
            var strBuilder = (StringBuilder)folder;
            var res = new ImportedDocInfo()
            {
                FileGuid = Guid.Empty,
                FileName = docInfo.FileName,
                FolderGuid = Guid.Empty,
                FolderPath = strBuilder.ToString()
            };

            await Task.Run(() =>
            {
                strBuilder.Append(docInfo.FileName);
                res.Url = strBuilder.ToString();
            });

            return res;
        }
    }
}
