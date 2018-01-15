using System.Threading.Tasks;

namespace Silk.Web.Core.Notifications
{
	public interface INotificationSendService<T>
	{
		Task SendNotificationAsync(T notification);
	}
}
