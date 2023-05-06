using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetApplicationAPI.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? Username { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? Email { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public string? PasswordHash { get; set; }

        [Required]
        [Column(TypeName = "varchar(45)")]
        public string? FirstName { get; set; }

        [Required]
        [Column(TypeName = "varchar(45)")]
        public string? LastName { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime CreatedOn { get; set; }

        [Required]
        [Column(TypeName = "datetime")]
        public DateTime LastLoginOn { get; set; }

        [Required]
        [Column(TypeName = "bit(1)")]
        public bool IsActive { get; set; }
    }
}
