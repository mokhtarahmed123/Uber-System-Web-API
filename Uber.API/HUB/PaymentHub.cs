using Microsoft.AspNetCore.SignalR;

namespace Uber.Uber
{
    public class PaymentHub:Hub
    {
        public async Task NotifyNewPayment(string customerEmail, int paymentId, string status)
        {
            await Clients.User(customerEmail).SendAsync("ReceiveNewPayment", paymentId, status);
        }

 
        public async Task UpdatePaymentStatus(string customerEmail, int paymentId, string status)
        {
            await Clients.User(customerEmail).SendAsync("PaymentStatusUpdated", paymentId, status);
        }

        public async Task BroadcastPaymentUpdate(int paymentId, string status)
        {
            await Clients.Group("Admins").SendAsync("PaymentUpdate", paymentId, status);
        }

    }
}
