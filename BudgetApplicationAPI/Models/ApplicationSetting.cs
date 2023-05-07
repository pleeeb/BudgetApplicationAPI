using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BudgetApplicationAPI.Models
{
    public class ApplicationSetting
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SettingId { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public int? Name { get; set; }

        [Required]
        [Column(TypeName = "varchar(250)")]
        public int? Value { get; set; }
    }
}
