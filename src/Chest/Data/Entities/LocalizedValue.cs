using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Chest.Data.Entities
{
    [Table("tb_localization", Schema = "chest")]
    public class LocalizedValue
    {
        [Required]
        [MaxLength(10)]
        [Column(Order = 0)]
        public string Locale { get; set; }
        
        [Required]
        [MaxLength(100)]
        [Column(Order = 1)]
        public string Key { get; set; }

        [Required]
        [MaxLength(4096)]
        [Column(Order = 2)]
        public string Value { get; set; }
    }
}