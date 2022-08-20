using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.Domain.Database.Models;

[Table("Documents", Schema = "SX")]
public class StorageDocument
{
    [Key]
    public int ID { get; set; }

    public string? DocNum { get; set; }
    public DateTime? DocDate { get; set; }
    public int CompanyId { get; set; }
    public string? ContractorName { get; set; }
    public string? ContractorEdrpou { get; set; }
    public string? ProcCode { get; set; }
    public int? ItemId { get; set; }
    public string? SharePointUrl { get; set; }
    public string? DocTypeName { get; set; }
    public string? MainDocNum { get; set; }
    public string? DocTitle { get; set; }
    public Guid? BusId { get; set; }
}
