using System.Threading.Tasks;

namespace Silk.Web.Core.Notifications
{
	public class EmailNotificationSender : INotificationSender
	{
		public string ProviderName => "Email";

		public Task SendAsync(string title, string notificationText)
		{
			return Task.CompletedTask;
		}
	}
}
