namespace MQ.DocImportService.Models.Import;

public class ContragentFolder : BasicFolder
{
    public string CaEgrp { get; init; }

    public ContragentFolder(string name, string caEgrp) : base(name)
    {
        CaEgrp = caEgrp;
    }
}
