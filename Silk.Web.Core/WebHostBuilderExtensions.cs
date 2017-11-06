using Microsoft.AspNetCore.Hosting;
using Silk.Web.Core.Components;

namespace Silk.Web.Core
{
	public static class WebHostBuilderExtensions
	{
		/// <summary>
		/// Add a component to be loaded during application startup.
		/// </summary>
		/// <typeparam name="TComponent"></typeparam>
		/// <param name="builder"></param>
		/// <returns></returns>
		public static IWebHostBuilder AddComponent<TComponent>(this IWebHostBuilder builder)
			where TComponent : IComponent, new()
		{
			SilkStartup.AddComponent(new ComponentInformation(
				new ReferencedComponentLoader<TComponent>()
				));
			return builder;
		}
	}
}
