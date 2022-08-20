using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MQ.DocImportService.Extensions;
using MQ.DocImportService.Models.Import;
using MQ.Domain.Database;
using MQ.Domain.Database.Models;
using MQ.Domain.Queue.Models;

namespace MQ.DocImportService.Services;

public sealed class DataroomImportService : IDocImportService
{
    private const string BasicDocsFolderName = "Господарсько-правові договори";
    private const string BasicDealDocType = "Договір";

    private readonly IDocStorageConnection _storageConnection;
    private readonly KernelDbContext _dbContext;

    public DataroomImportService(IDocStorageConnection storageConnection, KernelDbContext dbContext)
    {
        (_storageConnection, _dbContext) = (storageConnection, dbContext);
    }

    public async Task<ImportedDocInfo> ImportDocument(DocForDataroomQueueItem docInfo)
    {
        int companyId = _dbContext.GetCompanyId(docInfo.CompanyEgrp);

        var folder = await _storageConnection.CreateFolders(GetFoldersStructure(docInfo, companyId));
        if (folder is null)
            throw new ApplicationException($"Folders were not created, doc: {docInfo.DocType}, {docInfo.DocNum}");

        ImportedDocInfo importedInfo = await _storageConnection.UploadDocument(folder, docInfo);

        return importedInfo;
    }

    public async Task SaveImportedDocumentInfo(DocForDataroomQueueItem docInfo, ImportedDocInfo importedInfo)
    {
        if (docInfo.BusId is not null)
        {
            _dbContext.SignedDocs.Add(new SignedDocData().FillAttributes(docInfo, importedInfo));
        }

        int companyId = _dbContext.GetCompanyId(docInfo.CompanyEgrp);
        _dbContext.StorageDocuments.Add(new StorageDocument().FillAttributes(docInfo, importedInfo, companyId));

        await _dbContext.SaveChangesAsync();
    }

    public async Task ClearTempFiles(DocForDataroomQueueItem message)
    {
        await Task.Run(() => File.Delete(message.ContentPath));
    }

    private static List<BasicFolder> GetFoldersStructure(DocForDataroomQueueItem docInfo, int companyId)
    {
        List<BasicFolder> res = new()
        {
            new CompanyFolder($"cmp{companyId}", companyId, docInfo.CompanyEgrp),
            new BasicFolder(BasicDocsFolderName),
            new ContragentFolder($"{docInfo.ContragentName} ({docInfo.ContragentEgrp})", docInfo.ContragentEgrp)
        };

        if (docInfo.MainDocDate is not null && docInfo.MainDocNum is not null)
        {
            res.Add(new DocFolder(BasicDealDocType, docInfo.MainDocNum, docInfo.MainDocDate.Value));
            res.Add(new BasicFolder(docInfo.DocType));
        }

        res.Add(new DocFolder(docInfo.DocType, docInfo.DocNum, docInfo.DocDate));

        return res;
    }

    public void Dispose()
    {
        _storageConnection.Dispose();
        _dbContext.Dispose();
    }
}
