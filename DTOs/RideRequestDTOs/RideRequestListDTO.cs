namespace Uber.Uber.Application.DTOs.RideRequestDTOs
{
    public class RideRequestListDTO
    {
        public int Id { get; set; }
        public string RiderEmail { get; set; }
        public double PickupLat { get; set; }
        public double PickupLng { get; set; }
        public double DestinationLat { get; set; }
        public double DestinationLng { get; set; }
        public string RideRequestStatus { get; set; }
    }
}
