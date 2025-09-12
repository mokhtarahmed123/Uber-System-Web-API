using Uber.Uber.Domain.Entities.Enums;

namespace Uber.Uber.Application.DTOs.DeliveryDTOs
{
    public class DeliveryDetailsDTO
    {
        public int ID { get; set; }
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public double DropoffLat { get; set; }
        public double DropoffLng { get; set; }
        public DeliveryStatus Status { get; set; }
        public string DelivaryEmail { get; set; }
        public string DelivaryName { get; set; }
        public int TripID { get; set; }

    }
}
