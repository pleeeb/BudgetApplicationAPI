using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetApplicationAPI.Models
{
    public class LinkedBudget
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LinkId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int? BudgetId { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int? LinkedBudgetId { get; set; }
    }
}
