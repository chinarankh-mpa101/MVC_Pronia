namespace Pronia_example.Abstraction
{
	public interface IEmailService
	{
		Task SendEmailAsync(string email, string subject, string body);
	}
}
