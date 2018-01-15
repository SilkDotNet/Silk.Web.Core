using Silk.Web.Core.Components;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Routing;
using Silk.Web.Core.Constraints;
using Microsoft.AspNetCore.Authentication.Cookies;
using Silk.Web.Core.Data;
using Silk.Data.SQL.ORM;
using System;
using Silk.Web.Core.Services;
using Silk.Web.Core.Notifications;

namespace Silk.Web.Core
{
	/// <summary>
	/// Silk web application.
	/// </summary>
	public class WebApplication : IWebApplication
	{
		private readonly IServiceCollection _services;
		private DataDomain _dataDomain;

		public IServiceProvider ApplicationServices { get; internal set; }
		public LoadedComponent[] Components { get; }
		public ITypeLocator TypeLocator { get; }

		public WebApplication(IServiceCollection services,
			IEnumerable<LoadedComponent> loadedComponents)
		{
			_services = services;
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
			RegisterDefaultServices();
		}

		public void AddViewProvider(IFileProvider viewFileProvider)
		{
			_services.Configure<RazorViewEngineOptions>(razorOptions =>
			{
				razorOptions.FileProviders.Add(viewFileProvider);
			});
		}

		private void RegisterDefaultServices()
		{
			_services.Configure<RouteOptions>(options =>
			{
				options.ConstraintMap.Add("featureRoute", typeof(FeatureRouteConstraint));
			});
			_services.Configure<FeatureOptions>(options =>
			{
				options[CoreFeatures.UsernamePasswordSignInFeature].IsEnabled = true;
				options[CoreFeatures.UsernamePasswordSignInFeature].RouteUri = "signin";
				options[CoreFeatures.SignOutFeature].IsEnabled = true;
				options[CoreFeatures.SignOutFeature].RouteUri = "signout";
			});
			_services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
				.AddCookie(options =>
				{
					options.LoginPath = "/signin";
				});

			_services.AddTransient<DataDomain>(sP => _dataDomain);
			_services.AddSingleton<IFeaturesConfig, FeaturesConfigAccessor>();
			_services.AddTransient<IDatabase, SilkORMRootDatabase>();
			_services.AddTransient(typeof(IDatabase<>), typeof(SilkORMRootDatabase<>));
			_services.AddSingleton<ITokenGenerator, SecureTokenGenerator>();
			_services.AddScoped<UserManager>();

			_services.AddTransient(typeof(INotificationSendService<>), typeof(TemplatedNotificationSendService<>));
			_services.AddScoped<INotificationTemplateEngine, RazorNotificationTemplateEngine>();
		}

		public DataDomain BuildDataDomain()
		{
			var domainBuilder = new DataDomainBuilder();
			using (var scope = CreateServiceScope())
			{
				BuildDataDomain(scope.ServiceProvider, domainBuilder);
			}
			_dataDomain = domainBuilder.Build();
			return _dataDomain;
		}

		private void BuildDataDomain(IServiceProvider serviceProvider, DataDomainBuilder domainBuilder)
		{
			foreach (var dataComponent in GetDataComponents(serviceProvider))
			{
				dataComponent.InitializeDataDomainComponent(domainBuilder);
			}
		}

		private IEnumerable<IDataDomainComponent> GetDataComponents(IServiceProvider serviceProvider)
		{
			var targetInterfaceType = typeof(IDataDomainComponent);

			foreach (var serviceDescriptor in _services)
			{
				Type inspectType;
				if (serviceDescriptor.ImplementationInstance != null)
					inspectType = serviceDescriptor.ImplementationInstance.GetType();
				else if (serviceDescriptor.ImplementationType != null)
					inspectType = serviceDescriptor.ImplementationType;
				else if (serviceDescriptor.ServiceType != null)
					inspectType = serviceDescriptor.ServiceType;
				else
					continue;

				if (!inspectType.GetInterfaces().Contains(targetInterfaceType))
					continue;

				foreach (var service in serviceProvider.GetServices(serviceDescriptor.ServiceType))
				{
					if (service is IDataDomainComponent dataModelCollection)
						yield return dataModelCollection;
				}
			}
		}

		public IServiceScope CreateServiceScope()
		{
			var scope = ApplicationServices.GetRequiredService<IServiceScopeFactory>()
				.CreateScope();
			var scopedServiceProviderAccessor = ApplicationServices
				.GetRequiredService<ScopedServiceProviderAccessor>();
			scopedServiceProviderAccessor.CurrentScopedServiceProvider = scope.ServiceProvider;
			return scope;
		}
	}
}
