using Microsoft.AspNetCore.SignalR;

namespace Uber.Uber
{
    public class DeliveryHub:Hub
    {
        public async Task NotifyDriverNewDelivery(string driverEmail, int deliveryId)
        {
            await Clients.User(driverEmail)
                .SendAsync("ReceiveNewDelivery", deliveryId);
        }

        // إشعار للعميل بتحديث حالة Delivery
        public async Task UpdateDeliveryStatus(string customerEmail, int deliveryId, string status)
        {
            await Clients.User(customerEmail)
                .SendAsync("DeliveryStatusUpdated", deliveryId, status);
        }

        public async Task BroadcastDeliveryUpdate(int deliveryId, string status)
        {
            await Clients.Group("Admins")
                .SendAsync("DeliveryUpdate", deliveryId, status);
        }
    }
}
