using MailKit.Net.Smtp;
using MimeKit;
using Silk.Web.Core.Email;
using System.Threading.Tasks;

namespace Silk.Web.Core.Notifications
{
	public class EmailNotificationSender : INotificationSender
	{
		private readonly EmailOptions _emailOptions;

		public string ProviderName => "Email";

		public EmailNotificationSender(EmailOptions emailOptions)
		{
			_emailOptions = emailOptions;
		}

		public async Task SendAsync(string title, string notificationText, params UserAccount[] recipientAccounts)
		{
			var message = new MimeMessage();
			var bodyBuilder = new BodyBuilder
			{
				HtmlBody = notificationText
			};

			message.Subject = title;
			message.Body = bodyBuilder.ToMessageBody();
			message.From.Add(new MailboxAddress(_emailOptions.FromAddress.Name, _emailOptions.FromAddress.Address));
			foreach (var recipientAccount in recipientAccounts)
			{
				message.To.Add(new MailboxAddress(recipientAccount.EmailAddress));
			}

			using (var client = new SmtpClient())
			{
				client.ServerCertificateValidationCallback = (s, c, h, e) => true;

				await client.ConnectAsync(_emailOptions.ServerAddress, _emailOptions.ServerPort, _emailOptions.UseSSL)
					.ConfigureAwait(false);

				client.AuthenticationMechanisms.Remove("XOAUTH2");

				await client.AuthenticateAsync(_emailOptions.Username, _emailOptions.Password)
					.ConfigureAwait(false);

				await client.SendAsync(message)
					.ConfigureAwait(false);

				await client.DisconnectAsync(true)
					.ConfigureAwait(false);
			}
		}
	}
}
