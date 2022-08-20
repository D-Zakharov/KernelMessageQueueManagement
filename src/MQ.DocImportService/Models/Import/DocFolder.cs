namespace MQ.DocImportService.Models.Import;

public class DocFolder : BasicFolder
{
    public string DocType { get; set; }
    public string DocNum { get; set; }
    public DateTime DocDate { get; set; }

    public DocFolder(string docType, string docNum, DateTime docDate) : base($"{docType} №{docNum} від {docDate:dd.MM.yyyy}")
    {
        DocType = docType;
        DocNum = docNum;
        DocDate = docDate;
    }
}