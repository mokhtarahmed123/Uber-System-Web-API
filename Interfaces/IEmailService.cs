namespace Uber.Uber.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
