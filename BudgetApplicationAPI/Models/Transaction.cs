using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetApplicationAPI.Models
{
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TransactionId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int? UserId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int? BudgetId { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime TransactionDate { get; set; }

        [Required]
        [Column(TypeName = "varchar(45)")]
        public string? Category { get; set; }

        [Required]
        [Column(TypeName = "varchar(45)")]
        public string? Subcategory { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? Payee { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? Notes { get; set; }
    }
}
