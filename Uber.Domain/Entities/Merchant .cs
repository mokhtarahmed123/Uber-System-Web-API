using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Uber.Uber.Domain.Entities
{
    public class Merchant
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter the address.")]
        public string Address { get; set; }
            [Required]
            public double Latitude { get; set; }

            [Required]
            public double Longitude { get; set; }
        [Required]
        [ForeignKey("UserApp")]
        public string AppUserId { get; set; }
        public virtual User UserApp { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();

    }
}
