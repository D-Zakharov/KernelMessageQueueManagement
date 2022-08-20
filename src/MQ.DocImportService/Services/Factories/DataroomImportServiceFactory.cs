using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MQ.Domain.Database;

namespace MQ.DocImportService.Services.Factories
{
    public class DataroomImportServiceFactory : IDocImportServiceFactory
    {
        private readonly IDocStorageConnectionFactory docStorageFactory;
        private readonly IDbContextFactory<KernelDbContext> dbContextFactory;

        public DataroomImportServiceFactory(IDocStorageConnectionFactory docStorageFactory, IDbContextFactory<KernelDbContext> dbContextFactory)
        {
            this.docStorageFactory = docStorageFactory;
            this.dbContextFactory = dbContextFactory;
        }

        public IDocImportService GetService()
        {
            return new DataroomImportService(docStorageFactory.GetService(), dbContextFactory.CreateDbContext());
        }
    }
}
