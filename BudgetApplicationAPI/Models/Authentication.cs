using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetApplicationAPI.Models
{
    public class Authentication
    {
        [Key]
        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? AuthToken { get; set; }

        [Required]
        [Column(TypeName = "int")]
        public int? UserId { get; set; }
    }
}
