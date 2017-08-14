using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silk.Web.Core.Components;
using Silk.Web.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Web.Core
{
	/// <summary>
	/// Default Silk.Web.Core startup class.
	/// </summary>
	public class SilkStartup
	{
		private static readonly List<ComponentInformation> _configuredComponents = new List<ComponentInformation>();
		private static readonly List<Action<IServiceCollection>> _serviceRegisterDelegates = new List<Action<IServiceCollection>>();

		private static StartupPhase CurrentPhase { get; set; } = StartupPhase.HostBuilding;

		/// <summary>
		/// Add a component.
		/// </summary>
		/// <param name="componentInformation"></param>
		internal static void AddComponent(ComponentInformation componentInformation)
		{
			if (CurrentPhase > StartupPhase.ComponentDiscovery)
				throw new InvalidOperationException("Can not add a component after component discovery.");
			_configuredComponents.Add(componentInformation);
		}

		internal static void AddServiceRegistrationDelegate(Action<IServiceCollection> @delegate)
		{
			if (CurrentPhase > StartupPhase.ComponentDiscovery)
				throw new InvalidOperationException("Can not add service registration delegates after component discovery.");
			_serviceRegisterDelegates.Add(@delegate);
		}

		private IConfiguration _configuration;

		public SilkStartup(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		/// <summary>
		/// Discover Silk.Web components.
		/// </summary>
		/// <returns></returns>
		private IReadOnlyList<ComponentInformation> DiscoverComponents()
		{
			CurrentPhase = StartupPhase.ComponentDiscovery;
			var components = new List<ComponentInformation>(_configuredComponents);
			components.AddRange(
				FindComponents()
					.Where(newComponent => !components.Any(existingComponent => newComponent.Loader.Equals(existingComponent.Loader)))
				);
			return components;
		}

		/// <summary>
		/// Load components.
		/// </summary>
		/// <param name="components"></param>
		/// <returns></returns>
		private IReadOnlyList<LoadedComponent> LoadComponents(IReadOnlyList<ComponentInformation> components)
		{
			CurrentPhase = StartupPhase.ComponentCreation;
			var loadedComponents = new List<LoadedComponent>();
			foreach (var componentInformation in components)
			{
				if (ShouldLoadComponent(componentInformation))
				{
					loadedComponents.Add(new LoadedComponent(
						componentInformation,
						CreateComponentInstance(componentInformation)
						));
				}
			}
			return loadedComponents;
		}

		/// <summary>
		/// Create an instance of a component.
		/// </summary>
		/// <param name="componentInformation"></param>
		/// <returns></returns>
		private IComponent CreateComponentInstance(ComponentInformation componentInformation)
		{
			return componentInformation.Loader.CreateComponentInstance();
		}

		/// <summary>
		/// Called by aspnet core framework during webhost building.
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices(IServiceCollection services)
		{
			foreach (var serviceRegisterDelegate in _serviceRegisterDelegates)
			{
				serviceRegisterDelegate(services);
			}

			var components = DiscoverComponents();
			var loadedComponents = LoadComponents(components);
			var webApp = new WebApplication(loadedComponents);

			CurrentPhase = StartupPhase.ComponentInitalization;
			foreach (var componentInfo in webApp.Components)
				componentInfo.Component.Initalize(webApp);

			CurrentPhase = StartupPhase.ConfiguringServices;
			foreach (var componentInfo in webApp.Components)
				componentInfo.Component.ConfigureServices(services);

			services.AddSingleton<IFlatFileModelAssembler, FlatFileModelAssembler>();

			services.AddSingleton<IWebApplication>(webApp);
			services.AddMvc();

			//  install Silk.Signals
		}

		/// <summary>
		/// Called by aspnet core framework during webhost building.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			var webApp = app.ApplicationServices.GetRequiredService<IWebApplication>();

			//  configure any middleware for components
			CurrentPhase = StartupPhase.AddingMiddleware;

			foreach (var componentInfo in webApp.Components)
			{
				componentInfo.Component.AddMiddleware(app, env);
			}

			//  add MVC middleware
			app.UseMvc();

			CurrentPhase = StartupPhase.Startup;
			using (var scope = app.ApplicationServices.CreateScope())
			{
				new StartupHelper().FindAndExecute(scope.ServiceProvider, webApp);
			}

			CurrentPhase = StartupPhase.Running;
		}

		/// <summary>
		/// Decide if a component should be loaded or not.
		/// </summary>
		/// <param name="componentInformation"></param>
		/// <returns></returns>
		public virtual bool ShouldLoadComponent(ComponentInformation componentInformation)
		{
			return true;
		}

		/// <summary>
		/// Find components to load.
		/// </summary>
		/// <returns></returns>
		public virtual IEnumerable<ComponentInformation> FindComponents()
		{
			var componentLoaderBaseType = typeof(ReferencedComponentLoader<>);
			var componentTypes = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(assembly => assembly.GetTypes())
				.Where(type => IsLoadableComponent(type));
			foreach (var componentType in componentTypes)
			{
				var concreteLoaderType = componentLoaderBaseType.MakeGenericType(componentType);
				var componentLoader = Activator.CreateInstance(concreteLoaderType) as IComponentLoader;
				yield return new ComponentInformation(componentLoader);
			}
		}

		/// <summary>
		/// Determine if a type is safe to load as a component.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		protected virtual bool IsLoadableComponent(Type type)
		{
			if (type.IsInterface || type.IsAbstract || !type.GetInterfaces().Contains(typeof(IComponent)))
				return false;
			var ctor = type
				.GetConstructors(BindingFlags.Public | BindingFlags.CreateInstance | BindingFlags.Instance)
				.FirstOrDefault(constructor => constructor.GetParameters().Length == 0);
			return ctor != null;
		}
	}
}
