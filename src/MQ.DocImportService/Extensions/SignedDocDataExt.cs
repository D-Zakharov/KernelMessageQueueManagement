using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQ.DocImportService.Models.Import;
using MQ.Domain.Database.Models;
using MQ.Domain.Queue.Models;

namespace MQ.DocImportService.Extensions;

internal static class DocumentEntitiesExtensions
{
    internal static SignedDocData FillAttributes(this SignedDocData res, DocForDataroomQueueItem docInfo, ImportedDocInfo importedInfo)
    {
        res.DataroomFolderGuid = importedInfo.FolderGuid;
        res.CaEgrpou = docInfo.ContragentEgrp;
        res.DataroomGuid = importedInfo.FileGuid;
        res.DocDate = docInfo.DocDate;
        res.DocName = importedInfo.FileName;
        res.DocNum = docInfo.DocNum;
        res.DocType = docInfo.DocType;
        res.Folder = importedInfo.FolderPath;
        res.KernelEgrpou = docInfo.CompanyEgrp;
        res.MainDocDate = docInfo.MainDocDate;
        res.MainDocNum = docInfo.MainDocNum;
        res.ProjectCode = docInfo.ProjectCode;
        res.ReportBusId = docInfo.BusId;
        res.RequestId = docInfo.RequestId;

        return res;
    }

    internal static StorageDocument FillAttributes(this StorageDocument res,
        DocForDataroomQueueItem docInfo, ImportedDocInfo importedInfo, int companyId)
    {
        res.BusId = docInfo.BusId;
        res.CompanyId = companyId;
        res.ContractorEdrpou = docInfo.ContragentEgrp;
        res.ContractorName = docInfo.ContragentName;
        res.DocDate = docInfo.DocDate;
        res.DocNum = docInfo.DocNum;
        res.DocTitle = $"{docInfo.DocType} №{docInfo.DocNum} вiд {docInfo.DocDate:dd.MM.yyyy}";
        res.DocTypeName = docInfo.DocType;
        res.ItemId = docInfo.RequestId;
        res.MainDocNum = docInfo.MainDocNum;
        res.ProcCode = docInfo.ProjectCode;
        res.SharePointUrl = importedInfo.Url;

        return res;
    }
}
