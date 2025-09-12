namespace Uber.Uber.Application.DTOs.ComplaintsDTOs
{
    public class ComplaintDetailsDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string FromUserID { get; set; }
        public string AgainstUserId { get; set; }
        public int TripID { get; set; }
        public bool IsResolved { get; set; }

        public string FromUserName { get; set; }
        public string AgainstUserName { get; set; }
    }
}
