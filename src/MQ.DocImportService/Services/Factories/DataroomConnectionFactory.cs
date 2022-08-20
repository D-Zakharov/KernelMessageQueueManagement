using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using MQ.DocImportService.Models.Config;
using MQ.DocImportService.Models.Import;
using MQ.Domain.Queue.Models;

namespace MQ.DocImportService.Services.Factories
{
    public class DataroomConnectionFactory : IDocStorageConnectionFactory
    {
        private readonly IOptions<SharepointConfiguration> spOptions;

        public DataroomConnectionFactory(IOptions<SharepointConfiguration> spOptions)
        {
            this.spOptions = spOptions;
        }

        public IDocStorageConnection GetService()
        {
            return new DataroomConnection(spOptions);
        }
    }
}
