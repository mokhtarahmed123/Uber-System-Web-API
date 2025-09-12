using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Application
{
    public class ItemListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string MerchentEmail { get; set; }

    }
}
