using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Silk.Web.Core.Components
{
	/// <summary>
	/// Silk.Web component.
	/// </summary>
	public interface IComponent
	{
		/// <summary>
		/// Initialize the component.
		/// </summary>
		/// <param name="webApplication"></param>
		void Initalize(IWebApplication webApplication);

		/// <summary>
		/// Configure component services.
		/// </summary>
		/// <param name="services"></param>
		void ConfigureServices(IServiceCollection services);

		/// <summary>
		/// Add any middleware to the application pipeline.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		void AddMiddleware(IApplicationBuilder app, IHostingEnvironment env);
	}
}
