using System.Threading.Tasks;

namespace Silk.Web.Core.Notifications
{
	public interface INotificationTemplateEngine
	{
		Task<string> RenderAsync<TNotification>(TNotification notification, string templateDir);
	}
}
