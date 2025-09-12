using System.Threading.Tasks;
using Uber.Uber.Application.DTOs.PaymentDTOs;

namespace Uber.Uber.Application
{
    public interface IPaymentService
    {

        Task<CreatePaymentDTO> Create(CreatePaymentDTO request);
        Task<bool> Delete(int id);
        Task<UpdatePaymentDTO> Update(int id, UpdatePaymentDTO request);
        Task<PaymentDetailsDTO> GetByIdAsync(int id);
        Task<List<PaymentListDTO>> GetAllAsync();
        Task<List<PaymentListDTO>> SearchPaymentsAsync(SearchPaymentDTO search);
        Task<List<PaymentDetailsDTO>> GetPaymentDetailByCustomerEmail(string customerEmail);
        Task<List<PaymentDetailsDTO>> GetPaymentDetailByMerchantEmail(string MerchantEmail);

    }
}
