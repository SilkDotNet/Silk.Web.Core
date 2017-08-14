using Silk.Web.Core.Components;
using System.Linq;

namespace Silk.Web.Core
{
	public static class WebApplicationExtensionMethods
	{
		/// <summary>
		/// Get a component installed in the webapp.
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="webApp"></param>
		/// <returns></returns>
		public static TComponent GetComponent<TComponent>(this IWebApplication webApp)
			where TComponent : IComponent
		{
			return webApp.Components
				.Select(q => q.Component)
				.OfType<TComponent>()
				.FirstOrDefault();
		}
	}
}
