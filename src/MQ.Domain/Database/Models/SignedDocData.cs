using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MQ.Domain.Queue;

namespace MQ.Domain.Database.Models;

[Table("SignedDocsData", Schema = "SX")]
public class SignedDocData
{
    [Key]
    public int ID { get; set; }
    
    public Guid? ReportBusId { get; set; }
    public string? KernelEgrpou { get; set; }
    public string? CaEgrpou { get; set; }
    public DateTime? DocDate { get; set; }
    public string? DocNum { get; set; }
    public string? DocType { get; set; }
    public string? DocName { get; set; }
    public Guid? DataroomGuid { get; set; }
    public DateTime? MainDocDate { get; set; }
    public string? MainDocNum { get; set; }
    public string? Folder { get; set; }
    public Guid? DataroomFolderGuid { get; set; }
    public string? ProjectCode { get; set; }
    public int? RequestId { get; set; }
}
