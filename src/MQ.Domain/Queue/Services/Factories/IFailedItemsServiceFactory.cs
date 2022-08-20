namespace MQ.Domain.Queue.Services.Factories
{
    public interface IFailedItemsServiceFactory
    {
        FailedItemsDbService GetService();
    }
}