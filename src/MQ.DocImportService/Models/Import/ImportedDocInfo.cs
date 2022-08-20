namespace MQ.DocImportService.Models.Import;

public class ImportedDocInfo
{
    public Guid FolderGuid { get; set; }
    public string? FileName { get; set; }
    public string? Url { get; set; }
    public Guid FileGuid { get; set; }
    public string? FolderPath { get; set; }
}
