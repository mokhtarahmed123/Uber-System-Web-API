using Uber.Uber.Application;
using Uber.Uber.Application.DTOs.ComplaintsDTOs;

namespace Uber.Uber
{
    public interface IComplaintsService
    {
        Task<CreateComplaintsdto> CreateAsync(CreateComplaintsdto Create);
        Task<bool> DeleteAsync(int  id);
        Task<UpdateComplaintsdto> UpdateAsync(UpdateComplaintsdto Update,int id);
        Task<ComplaintDetailsDTO> GetComplaintByIdAsync(int id);
        Task<List<ListComplaintsDTO>> GetAllComplaintsAsync();
        Task<List<ListComplaintsDTO>> SearchComplaintsAsync(SearchComplaintDTO Search);
        Task<bool> ResolveComplaintAsync(int id, ResolveComplaintDTO resolveDto);

        Task<List<ListComplaintsDTO>> GetComplaintByCustomer();
    }
}
