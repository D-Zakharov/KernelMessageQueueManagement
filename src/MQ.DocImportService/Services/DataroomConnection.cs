using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SharePoint.Client;
using Microsoft.Extensions.Options;
using System.Security;
using MQ.DocImportService.Models.Config;
using MQ.DocImportService.Models.Import;
using MQ.Domain.Queue.Models;

namespace MQ.DocImportService.Services;

public sealed class DataroomConnection : IDocStorageConnection
{
    private const string UnacceptableChars = "\"*:<>?/\\|\r\n\t";
    private const int MaxTriesCountForSafeLoad = 10;

    private readonly SharepointConfiguration _spOptions;
    private readonly ClientContext _Context;
    private readonly Web _Web;

    public DataroomConnection(IOptions<SharepointConfiguration> spOptions)
    {
        _spOptions = spOptions.Value;
        _spOptions.CheckData();

        _Context = new ClientContext(_spOptions.SiteUrl);
        _Context.DisableReturnValueCache = true;

        var password = new SecureString();
        foreach (char ch in _spOptions.Password!)
            password.AppendChar(ch);

        _Context.Credentials = new SharePointOnlineCredentials(_spOptions.Login, password);
        _Web = _Context.Web;
        _Context.Load(_Web);
        _Context.ExecuteQuery();
    }

    public async Task<object?> CreateFolders(List<BasicFolder> folders)
    {
        CompanyFolder companyFolder = await Task.Run(() => GetCompanyFolder(folders));
        List companyList = await Task.Run(() => GetSharepointCompanyLibrary(companyFolder));
        var parentRelativePath = new StringBuilder($"{companyList.Context.Url}/{companyFolder.Name}");

        Folder? res = null;
        foreach (var folder in folders)
        {
            folder.Name = FixNameForSharepoint(folder.Name);
            res = await Task.Run(() => CreateFolder(companyList, res, parentRelativePath.ToString(), folder));
            parentRelativePath.Append($"/{folder.Name}");
        }
        return res;
    }

    public void Dispose()
    {
        if (_Context is not null)
            _Context.Dispose();
    }

    private Folder? CreateFolder(List companyList, Folder? parentFolder, string relativePath, BasicFolder folder)
    {
        return folder switch
        {
            CompanyFolder => null, //это библиотека, она не создается в рамках данного сервиса
            ContragentFolder => CreateContragentFolder(companyList, parentFolder, relativePath, (ContragentFolder)folder),
            DocFolder => CreateBasicFolder(companyList, relativePath, folder.Name),
            BasicFolder => CreateBasicFolder(companyList, relativePath, folder.Name),
            _ => CreateBasicFolder(companyList, relativePath, folder.Name),
        };
    }

    public async Task<ImportedDocInfo> UploadDocument(object folder, DocForDataroomQueueItem docInfo)
    {
        var agreementFolder = (Folder)folder;

        using (var fileStream = new FileStream(docInfo.ContentPath, FileMode.Open))
        {
            var creationInfo = new FileCreationInformation()
            {
                ContentStream = fileStream,
                Overwrite = true,
                Url = FixNameForSharepoint(docInfo.FileName)
            };

            var file = agreementFolder.Files.Add(creationInfo);
            file.Update();
            await Task.Run(() => Load(file));

            if (docInfo.LinkToAgreement is not null || docInfo.BusId is not null)
            {
                if (docInfo.BusId.HasValue)
                    file.ListItemAllFields["BusId"] = docInfo.BusId.ToString();
                if (!string.IsNullOrEmpty(docInfo.LinkToAgreement))
                    file.ListItemAllFields["LinkToAgreement"] = docInfo.LinkToAgreement;

                file.ListItemAllFields.Update();
                await Task.Run(() => _Context.ExecuteQuery());
            }

            return new ImportedDocInfo()
            {
                FileGuid = file.UniqueId,
                FolderGuid = agreementFolder.UniqueId,
                FileName = creationInfo.Url,
                FolderPath = file.ServerRelativeUrl,
                Url = new Uri(_Context.Url).GetLeftPart(UriPartial.Authority) + file.ServerRelativeUrl
            };
        }
    }

    private Folder? CreateContragentFolder(List companyList, Folder? parentFolder, string relativePath, ContragentFolder folder)
    {
        if (parentFolder is null)
            throw new ApplicationException($"Cannot create CA folder '{folder.Name}' because parent folder is null");

        string indexedAttrValue = folder.CaEgrp == "нерезидент" ? folder.Name : folder.CaEgrp;
        var res = FindExistingContragentFolder(companyList, parentFolder, indexedAttrValue);
        if (res is null)
        {
            res = CreateBasicFolder(companyList, relativePath, folder.Name);

            var attrs = Load(res.ListItemAllFields);
            attrs["ContractorEDRPOU"] = indexedAttrValue;
            attrs.Update();
            SafeLoad(res, false);
            return res;
        }

        return res;
    }

    private Folder? FindExistingContragentFolder(List companyList, Folder parentFolder, string indexedAttrValue)
    {
        string filter = $"<Query><Where><Eq><FieldRef Name='ContractorEDRPOU' /><Value Type='Text'>{indexedAttrValue}</Value></Eq></Where></Query>";

        var camlQuery = new CamlQuery
        {
            FolderServerRelativeUrl = parentFolder.ServerRelativeUrl,
            ViewXml = $"<View>{filter}<RowLimit>1</RowLimit></View>"
        };

        ListItemCollection items = companyList.GetItems(camlQuery);
        SafeLoad(items);
        if (items.Count > 0)
        {
            Folder caFld = items[0].Folder;
            SafeLoad(caFld);
            return caFld;
        }
        return null;
    }

    private Folder CreateBasicFolder(List companyList, string relativePath, string folderName)
    {
        Folder? res = GetFolder(relativePath, folderName);
        if (res == null)
        {
            var itemCreateInfo = new ListItemCreationInformation();
            itemCreateInfo.UnderlyingObjectType = FileSystemObjectType.Folder;
            itemCreateInfo.LeafName = folderName;
            itemCreateInfo.FolderUrl = relativePath;

            ListItem newItem = companyList.AddItem(itemCreateInfo);
            newItem["Title"] = folderName;
            newItem.Update();
            _Context.ExecuteQuery();

            res = Load(newItem.Folder);

            return res;
        }
        return res;
    }

    private Folder? GetFolder(string relativePath, string folderName)
    {
        Folder? res = _Web.GetFolderByServerRelativeUrl($"{relativePath}/{folderName}");
        _Web.Context.Load(res);
        try
        {
            _Web.Context.ExecuteQuery();
            return res;
        }
        catch (ServerException) { }

        return null;
    }

    private static CompanyFolder GetCompanyFolder(List<BasicFolder> folders)
    {
        var companyFolder = folders.FirstOrDefault(i => i is CompanyFolder);
        if (companyFolder is null)
            throw new ApplicationException("Company folder is missing in folders list");

        return (CompanyFolder)companyFolder;
    }

    private List GetSharepointCompanyLibrary(CompanyFolder companyFolder)
    {
        return Load(_Web.GetList($"{_spOptions}/sites/dataroom/cmp{companyFolder.CompanyId}"));
    }

    private T Load<T>(T clientObject) where T : ClientObject
    {
        _Context.Load(clientObject);
        _Context.ExecuteQuery();
        return clientObject;
    }

    private T SafeLoad<T>(T clientObject, bool execLoad = true) where T : ClientObject
    {
        bool isLoaded = false;
        int counter = MaxTriesCountForSafeLoad;
        while (!isLoaded && counter > 0)
        {
            try
            {
                if (execLoad)
                    _Context.Load(clientObject);

                _Context.ExecuteQuery();
                isLoaded = true;
            }
            catch
            {
                counter--;
                if (counter == 0)
                    throw;
                Thread.Sleep(100);
            }
        }
        return clientObject;
    }

    private static string FixNameForSharepoint(string name)
    {
        var resBuffer = new char[name.Length];
        int counter = 0;
        foreach (char ch in name.Trim())
        {
            if (!UnacceptableChars.Contains(ch))
                resBuffer[counter++] = ch;
        }

        ReadOnlySpan<char> res = resBuffer.AsSpan()[..counter];
        return res.ToString();
    }
}
