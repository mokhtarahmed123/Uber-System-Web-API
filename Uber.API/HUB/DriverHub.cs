using Microsoft.AspNetCore.SignalR;

namespace Uber.Uber
{
    public class DriverHub:Hub
    {
        public async Task UpdateDriverStatus(string driverEmail, string status)
        {
            await Clients.All.SendAsync("DriverStatusUpdated", driverEmail, status);
        }

        public async Task UpdateDriverProfile(string driverEmail)
        {
            await Clients.User(driverEmail)
                .SendAsync("DriverProfileUpdated", driverEmail);
        }
    }
}
