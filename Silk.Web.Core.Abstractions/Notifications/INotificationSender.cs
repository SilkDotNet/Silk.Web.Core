using System.Threading.Tasks;

namespace Silk.Web.Core.Notifications
{
	public interface INotificationSender
	{
		string ProviderName { get; }
		Task SendAsync(string title, string notificationText, params UserAccount[] recipientAccounts);
	}
}
