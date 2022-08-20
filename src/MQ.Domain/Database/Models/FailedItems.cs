using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.Domain.Database.Models
{
    [Table("FailedItems", Schema = "SXQUEUE")]
    public class FailedItem
    {
        public int Id { get; set; }
        public ItemTypes ItemType { get; set; }
        public DateTime CreationDate { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? SerializedItem { get; set; }
    }

    public enum ItemTypes { DocImport }
}
