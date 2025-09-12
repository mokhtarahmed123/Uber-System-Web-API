using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application.DTOs.CategoryDTO
{
    public class GetListOfItemsInCategory
    {
        public int Id { get; set; }
        public string Name { get; set; }
          public int CountOfItem {  get; set; }
        public List<Item> Items { get; set; }

    }
}
