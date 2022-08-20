using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MQ.Domain.Database.Models;

namespace MQ.Domain.Database
{
    public class KernelDbContext : DbContext
    {
        public virtual DbSet<KernelCompany> KernelCompanies { get; set; } = null!;
        public virtual DbSet<IdempotencyKey> IdempotencyKeys { get; set; } = null!;
        public virtual DbSet<SignedDocData> SignedDocs { get; set; } = null!;
        public virtual DbSet<StorageDocument> StorageDocuments { get; set; } = null!;
        public virtual DbSet<FailedItem> FailedItems { get; set; } = null!;

        public KernelDbContext(DbContextOptions<KernelDbContext> options) : base(options)
        {
        }

        public int GetCompanyId(string companyEgrp)
        {
            var company = KernelCompanies.FirstOrDefault(i => i.EDRPOU == companyEgrp && string.IsNullOrEmpty(i.Branch));
            if (company == null)
                throw new KeyNotFoundException($"Company with EGRP {companyEgrp} does not exist");

            return company.ID;
        }
    }
}
