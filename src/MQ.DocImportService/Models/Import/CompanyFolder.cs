namespace MQ.DocImportService.Models.Import;

public class CompanyFolder : BasicFolder
{
    public int CompanyId { get; set; }
    public string CompanyEgrp { get; set; }

    public CompanyFolder(string name, int companyId, string companyEgrp) : base(name)
    {
        (CompanyId, CompanyEgrp) = (companyId, companyEgrp);
    }
}
