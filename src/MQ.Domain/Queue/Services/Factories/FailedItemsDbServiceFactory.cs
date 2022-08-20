using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MQ.Domain.Database;

namespace MQ.Domain.Queue.Services.Factories
{
    public class FailedItemsDbServiceFactory : IFailedItemsServiceFactory
    {
        private readonly IDbContextFactory<KernelDbContext> dbContextFactory;

        public FailedItemsDbServiceFactory(IDbContextFactory<KernelDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public FailedItemsDbService GetService()
        {
            return new FailedItemsDbService(dbContextFactory.CreateDbContext());
        }
    }
}
