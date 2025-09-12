using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Uber.Uber
{
    public class TripHub : Hub
    {


        public async Task NotifyDriverNewTrip(string DriverEmail, int TripId)
        {
            await Clients.User(DriverEmail).SendAsync("ReceiveNewTrip", TripId);
        }

        public async Task UpdateTripStatus(string customerEmail, int tripId, string status)
        {
            await Clients.User(customerEmail).SendAsync("TripStatusUpdated", tripId, status);
        }
        public async Task BroadcastTripUpdate(int tripId, string status)
        {
            await Clients.Group("Admins").SendAsync("TripUpdate", tripId, status);
        }
    }
}
