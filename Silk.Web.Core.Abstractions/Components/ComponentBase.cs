using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace Silk.Web.Core.Components
{
	/// <summary>
	/// Simple component base class.
	/// Useful for component authors that just want to implement methods they need.
	/// </summary>
	public abstract class ComponentBase : IComponent
	{
		/// <summary>
		/// Gets the web application instance.
		/// </summary>
		protected IWebApplication WebApplication { get; set; }

		public virtual void Initalize(IWebApplication webApplication)
		{
			WebApplication = webApplication;
		}

		public virtual void ConfigureServices(IServiceCollection services)
		{
		}

		public virtual void AddMiddleware(IApplicationBuilder app, IHostingEnvironment env)
		{
		}
	}
}
