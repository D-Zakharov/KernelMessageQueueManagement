namespace MQ.DocImportService.Services.Factories
{
    public interface IDocStorageConnectionFactory
    {
        IDocStorageConnection GetService();
    }
}