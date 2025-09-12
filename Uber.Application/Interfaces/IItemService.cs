namespace Uber.Uber.Application
{
    public interface IItemService
    {
        Task<CreateandUpdateItemDTO> CreateItemAsync(CreateandUpdateItemDTO itemDTO);
        Task<CreateandUpdateItemDTO> UpdateItemAsync(int id, CreateandUpdateItemDTO itemDTO);
        Task<GetItemDTO> GetItemById(int id);
        Task<bool> DeleteItem(int id);
        Task<List<ItemListDTO>> GetAllItems();
        Task<List<ItemListDTO>> GetItemsByCategory(string categoryName);
        Task<List<ItemListDTO>> GetItemsByMerchant(string merchantEmail);

    }
}
