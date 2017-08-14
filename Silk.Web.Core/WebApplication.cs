using Silk.Web.Core.Components;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Web.Core
{
	/// <summary>
	/// Silk web application.
	/// </summary>
	public class WebApplication : IWebApplication
	{
		public LoadedComponent[] Components { get; }
		public ITypeLocator TypeLocator { get; }

		public WebApplication(IEnumerable<LoadedComponent> loadedComponents)
		{
			Components = loadedComponents.ToArray();
			TypeLocator = new TypeLocator(
				new[] {
					Assembly.GetEntryAssembly(),
					typeof(WebApplication).Assembly,
					typeof(IWebApplication).Assembly
				}.Concat(
					Components
					.Select(q => q.Component.GetType().Assembly)
				)
				.GroupBy(q => q)
				.Select(q => q.First())
			);
		}
	}
}
