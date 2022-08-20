namespace MQ.DocImportService.Services.Factories
{
    public interface IDocImportServiceFactory
    {
        IDocImportService GetService();
    }
}