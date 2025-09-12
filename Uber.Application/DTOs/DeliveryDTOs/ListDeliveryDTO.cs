namespace Uber.Uber.Application
{
    public class ListDeliveryDTO
    {
        public int Id { get; set; }
        public string DriverEmail { get; set; }
        public string Status { get; set; }
        public string DelivaryName { get; set; }
        public int TripId {  get; set; }
    }
}
