using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MQ.Domain.Database.Models
{
    [Table("Companies", Schema = "SX")]
    public class KernelCompany
    {
        public int ID { get; set; }
        public string EDRPOU { get; set; } = default!;
        public string ShortName { get; set; } = default!;
        public string? Branch { get; set; }
    }
}
