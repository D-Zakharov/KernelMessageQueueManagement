namespace MQ.DocImportService.Models.Import;

public interface IStorageFolder { }

public class BasicFolder : IStorageFolder
{
    public string Name { get; set; }

    public BasicFolder(string name)
    {
        Name = name;
    }
}
