using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetApplicationAPI.Models
{
    public class Budget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BudgetId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int? UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(45)")]
        public string? Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? Description { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal? Amount { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime StartDate { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime EndDate { get; set; }
    }
}
