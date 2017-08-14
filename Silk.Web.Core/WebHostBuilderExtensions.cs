using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Silk.Web.Core.Components;
using System;

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

		/// <summary>
		/// Register services for dependency injection.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="serviceDelegate"></param>
		/// <returns></returns>
		public static IWebHostBuilder WithServices(this IWebHostBuilder builder, Action<IServiceCollection> serviceDelegate)
		{
			return builder;
		}
	}
}
