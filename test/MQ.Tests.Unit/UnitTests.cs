using System.Text;
using Microsoft.EntityFrameworkCore;
using Moq;
using MQ.DataroomImportApi.Models;
using MQ.DocImportService.Models.Import;
using MQ.DocImportService.Services;
using MQ.Domain.Database;
using MQ.Domain.Database.Models;
using MQ.Domain.Queue;
using MQ.Domain.Queue.Models;
using MQ.Domain.Rabbit;
using MQ.Tests.Unit.Mock.Service;

namespace MQ.Tests.Unit;

public class UnitTests
{
    private const string TestKernelEgrp = "31413141";
    private const string TestContragentEgrp = "00413141";
    private const string TestContragentName = "Ca name";
    private const string TestDocNum = "qwert-3456";
    private const string TestDocType = "Invoice";
    private const string TestMainDocNum = "asdfg-789";

    private readonly Mock<KernelDbContext> _mockDbCtx;
    private readonly IDocStorageConnection _storageConnection;

    public UnitTests()
    {
        _storageConnection = new MockDocStorageConnection();

        var companiesData = new List<KernelCompany>()
        {
            new KernelCompany { ID = 1, EDRPOU = TestKernelEgrp, ShortName= "Mock Kernel Company" }
        };

        var signedDocsData = new List<SignedDocData>();
        var storageDocsData = new List<StorageDocument>();

        _mockDbCtx = new Mock<KernelDbContext>(new DbContextOptions<KernelDbContext>());
        _mockDbCtx.Setup(c => c.KernelCompanies).Returns(CreateQueryableMockDbSet(companiesData));
        _mockDbCtx.Setup(c => c.SignedDocs).Returns(CreateQueryableMockDbSet(signedDocsData));
        _mockDbCtx.Setup(c => c.StorageDocuments).Returns(CreateQueryableMockDbSet(storageDocsData));
    }

    [Fact]
    public void Should_Throw_On_Null_Username()
    {
        var config = new RabbitConfiguration()
        {
            DocsQueueName = "a", Host = "s", Pass = "s", Username = null
        };

         var exception = Assert.Throws<ArgumentNullException>(
            () => config.CheckBasicProperties()
        );

        Assert.Equal($"{nameof(config.Username)}", exception.ParamName);
    }

    [Fact]
    public void Should_Throw_On_Empty_Username()
    {
        var config = new RabbitConfiguration()
        {
            DocsQueueName = "a",
            Host = "s",
            Pass = "s",
            Username = ""
        };

        var exception = Assert.Throws<ArgumentNullException>(
           () => config.CheckBasicProperties()
       );

        Assert.Equal($"{nameof(config.Username)}", exception.ParamName);
    }

    [Fact]
    public async Task Should_Create_Mock_Dataroom_Folders_And_File()
    {
        var service = new DataroomImportService(storageConnection: _storageConnection, dbContext: _mockDbCtx.Object);

        string filePath = $"{Path.GetTempPath()}/temp{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
        File.WriteAllText(filePath, "temp file");
        var testData = new DocForDataroomQueueItem() 
        {
            BusId = Guid.NewGuid(), 
            CompanyEgrp = TestKernelEgrp, 
            ContentPath = filePath, 
            ContragentEgrp = TestContragentEgrp, 
            ContragentName = TestContragentName, 
            DocDate = DateTime.Now,
            DocNum = TestDocNum, 
            DocType = TestDocType, 
            LinkToAgreement = null, 
            MainDocDate = DateTime.Now.AddDays(-1), 
            MainDocNum = TestMainDocNum, 
            ProjectCode = "test", 
            RequestId = 0
        };

        var res = await service.ImportDocument(testData);

        Assert.Equal($"cmp1/Господарсько-правові договори/{TestContragentName} ({TestContragentEgrp})/{new DocFolder("Договір", testData.MainDocNum, testData.MainDocDate.Value).Name}/{TestDocType}/{new DocFolder(TestDocType, testData.DocNum, testData.DocDate).Name}/{testData.FileName}", res.Url);
    }

    private static DbSet<T> CreateQueryableMockDbSet<T>(List<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();

        var dbSet = new Mock<DbSet<T>>();
        dbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        dbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        dbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        dbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
        dbSet.Setup(i => i.Add(It.IsAny<T>())).Callback<T>((j) => sourceList.Add(j));

        return dbSet.Object;
    }
}