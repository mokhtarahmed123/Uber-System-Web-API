using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Domain.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]

        public string Name { get; set; }
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    }
}
