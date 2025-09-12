
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Uber.Uber.Domain.Entities;

namespace Uber.Uber.Application
{
    public class ItemService : IItemService
    {
        private readonly IItemRepo itemRepo;
        private readonly UberContext context;
        private readonly IMapper mapper;
        private readonly ILogger<Item> logger;

        public ItemService(IItemRepo itemRepo, UberContext context, IMapper mapper, ILogger<Item> logger)
        {
            this.itemRepo = itemRepo;
            this.context = context;
            this.mapper = mapper;
            this.logger = logger;
        }
        public async Task<CreateandUpdateItemDTO> CreateItemAsync(CreateandUpdateItemDTO itemDTO)
        {
            if (itemDTO == null)
                throw new ArgumentNullException(nameof(itemDTO));

            var category = await context.Categories
                .FirstOrDefaultAsync(c => c.Name == itemDTO.CategoryName);

            if (category == null)
                throw new KeyNotFoundException($"Category '{itemDTO.CategoryName}' not found.");

            var merchant = await context.Merchants
                .Include(m => m.UserApp)
                .FirstOrDefaultAsync(m => m.UserApp.Email == itemDTO.MerchantEmail);

            if (merchant == null)
                throw new KeyNotFoundException($"Merchant '{itemDTO.MerchantEmail}' not found.");

            var item = mapper.Map<Item>(itemDTO);

            item.CategoryId = category.Id;
            item.MerchantID = merchant.Id;

            await itemRepo.Create(item);

            return mapper.Map<CreateandUpdateItemDTO>(item);
        }



        public async Task<bool> DeleteItem(int id)
        {
            var item = await itemRepo.GetByID(id);
            if (item == null)
            {
                logger.LogWarning("Item with ID {Id} not found.", id);
                return false;
            }

            await itemRepo.Delete(id);
            return true;
        }


        public async Task<List<ItemListDTO>> GetAllItems()
        {
            var list = await itemRepo.FindAll();
            var mappedList = mapper.Map<List<ItemListDTO>>(list);
            return mappedList;

        }

        public async Task<GetItemDTO> GetItemById(int id)
        {
            var Item = await itemRepo.GetByID(id);
            return mapper.Map<GetItemDTO>(Item);
        }

        public async Task<List<ItemListDTO>> GetItemsByCategory(string categoryName)
        {
            var List = await itemRepo.GetItemsByCategory(categoryName);
            var mappedList = mapper.Map<List<ItemListDTO>>(List);
            return mappedList;

        }

        public async Task<List<ItemListDTO>> GetItemsByMerchant(string merchantEmail)
        {
            var List = await itemRepo.GetItemsByMerchant(merchantEmail);
            var mappedList = mapper.Map<List<ItemListDTO>>(List);
            return mappedList;

        }

        public async Task<CreateandUpdateItemDTO> UpdateItemAsync(int id, CreateandUpdateItemDTO itemDTO)
        {
            if (id <= 0)
                throw new ArgumentException("Invalid item ID.");

            var existingItem = await itemRepo.GetByID(id);
            if (existingItem == null)
                throw new KeyNotFoundException($"Item with ID {id} not found.");

            var category = await context.Categories
                .FirstOrDefaultAsync(c => c.Name == itemDTO.CategoryName);

            if (category == null)
                throw new KeyNotFoundException($"Category '{itemDTO.CategoryName}' not found.");

            var merchant = await context.Merchants
                .Include(m => m.UserApp)
                .FirstOrDefaultAsync(m => m.UserApp.Email == itemDTO.MerchantEmail);

            if (merchant == null)
                throw new KeyNotFoundException($"Merchant '{itemDTO.MerchantEmail}' not found.");

            mapper.Map(itemDTO, existingItem);
            existingItem.CategoryId = category.Id;
            existingItem.MerchantID = merchant.Id;

            await itemRepo.Update(id, existingItem);

            return mapper.Map<CreateandUpdateItemDTO>(existingItem);
        }
    }
}
