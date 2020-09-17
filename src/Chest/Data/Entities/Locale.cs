using System.ComponentModel.DataAnnotations.Schema;

namespace Chest.Data.Entities
{
    [Table("tb_locales", Schema = "chest")]
    public class Locale
    {
        public string Id { get; set; }
        public bool IsDefault { get; set; }
    }
}