using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MQ.Domain.Database.Models
{
    [Table("IdempotencyKeys", Schema = "SX")]
    public class IdempotencyKey
    {
        [Key]
        public Guid Id { get; set; }

        public DateTime CreationDate { get; set; }

        public IdempotencyProjectCodes ProjectCode { get; set; }
    }

    public enum IdempotencyProjectCodes { ShipmentActs }
}
