using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Silk.Data.SQL.ORM;
using Silk.Data.SQL.Providers;
using Silk.Data.SQL.SQLite3;
using Silk.Web.Core.Components;
using Silk.Web.Core.Installation;
using Silk.Web.Core.Persistence;
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

		private static StartupPhase CurrentPhase { get; set; } = StartupPhase.HostBuilding;

		static SilkStartup()
		{
			_configuredComponents.Add(
				new ComponentInformation(
					new ReferencedComponentLoader<InstallerComponent>()
				)
			);
		}

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
			var components = DiscoverComponents();
			var loadedComponents = LoadComponents(components);
			var webApp = new WebApplication(services, loadedComponents);

			CurrentPhase = StartupPhase.ComponentInitalization;
			foreach (var componentInfo in webApp.Components)
				componentInfo.Component.Initalize(webApp);

			CurrentPhase = StartupPhase.ConfiguringServices;
			foreach (var componentInfo in webApp.Components)
				componentInfo.Component.ConfigureServices(services);

			services.AddSingleton<IWebApplication>(webApp);
			services.AddMvc().ConfigureApplicationPartManager(manager =>
				manager.ApplicationParts.Insert(0, new AssemblyPart(Assembly.GetEntryAssembly())));
			webApp.AddEmbeddedViewProvider(typeof(WebApplication).Assembly);

			services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			services.AddSingleton<ScopedServiceProviderAccessor>();
			services.AddSingleton<IScopedServiceProviderAccessor>(sP => sP.GetRequiredService<ScopedServiceProviderAccessor>());

			services.AddSingleton<IDataProvider>(new SQLite3DataProvider("silk-db.sqlite", nonBinaryGUIDs: true));

			services.AddScoped<IAccountRepository<UserAccount>, DbAccountRepository<UserAccount>>();
			services.AddScoped<ICredentialsManager<UsernamePasswordCredentials, UserAccount>,
				DbUserPasswordCredentialsManager<UserAccount>>();
			services.AddScoped<IAccountRolesRepository, DbAccountRolesRepository>();

			//  install Silk.Signals
		}

		/// <summary>
		/// Called by aspnet core framework during webhost building.
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure(IApplicationBuilder app, IHostingEnvironment env)
		{
			var webApp = app.ApplicationServices.GetRequiredService<IWebApplication>()
				as WebApplication;
			webApp.ApplicationServices = app.ApplicationServices;

			//  ensure DataDomain has been built, do NOT remove this even though it's not used anywhere here
			var dataDomain = webApp.BuildDataDomain();

			//  configure any middleware for components
			CurrentPhase = StartupPhase.AddingMiddleware;

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseAuthentication();

			foreach (var componentInfo in webApp.Components)
			{
				componentInfo.Component.AddMiddleware(app, env);
			}

			app.UseStaticFiles();
			app.UseMvcWithDefaultRoute();

			CurrentPhase = StartupPhase.Startup;
			using (var scope = webApp.CreateServiceScope())
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
