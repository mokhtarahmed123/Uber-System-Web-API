using System.ComponentModel.DataAnnotations;

namespace Uber.Uber.Application
{
    public class ListComplaintsDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string FromUserID { get; set; }
        public string AgainstUserId { get; set; }
        public int TripID { get; set; }
        public bool IsResolved { get; set; }

    }
}
