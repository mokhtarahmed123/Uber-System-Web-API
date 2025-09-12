using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application
{
    public class GetDeliveryByStatus
    {
        public DeliveryStatus DeliveryStatus { get; set; }
        public int DeliveryId { get; set; }
        public int TripId {  get; set; }
    }
}
