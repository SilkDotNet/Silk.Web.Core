using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Silk.Web.Core.Abstractions;
using Silk.Web.Core.Abstractions.ViewModels;
using Silk.Mapping;
using Silk.Signals;
using System;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Silk.Web.Core.Menus;

namespace Silk.Web.Core
{
	public static class StartupExtensions
	{
		private static WebBuilder WebBuilder { get; set; }
		private static IWebConfig WebConfig { get; set; }

		public static IWebBuilder AddSilkWebCore(this IServiceCollection serviceCollection)
		{
			if (WebBuilder != null)
				return WebBuilder;

			WebBuilder = new WebBuilder(serviceCollection);
			WebBuilder.SetSite(new Site("My Site", "", "http://localhost/"));

			serviceCollection.AddOptions();
			serviceCollection.AddSingleton<IConfigureOptions<WebOptions>, WebOptionsConfigure>();

			serviceCollection
				.AddSilkMappingServices()
				.Configure<MappingOptions>(options =>
				{
					options.AutoDiscover(Assembly.GetEntryAssembly());
					options.AutoDiscover(typeof(StartupExtensions).GetTypeInfo().Assembly);
				});

			serviceCollection.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			serviceCollection.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
			serviceCollection.AddSingleton<ISiteRepository>(new InMemorySiteRepository(WebBuilder));
			serviceCollection.AddSingleton<ISiteResolver, HostnameSiteResolver>();
			serviceCollection.AddSingleton<ISiteResolver>(new DefaultSiteResolver(WebBuilder));
			serviceCollection.AddScoped<IWebContextFactory, WebContextFactory>();
			serviceCollection.AddScoped<WebContextState>();
			serviceCollection.AddScoped<IWebContext>(WebContextFactory);
			serviceCollection.AddScoped<GlobalViewModel>(GlobalViewModelFactory);
			serviceCollection.AddScoped<IMenuBuilder, MenuBuilder>();
			serviceCollection.AddSingleton<IAccountRepository<UserAccount>, InMemoryAccountRepository<UserAccount>>();
			serviceCollection.AddSingleton<IDashboardProvider, DashboardProvider>();
			serviceCollection.AddScoped<IMenuProvider, DashboardMenuProvider>();
			serviceCollection.AddSingleton<IWebConfig>(sP => WebConfig);
			serviceCollection.AddScoped<IAccountAccessor<UserAccount>, UserAccountAccessor>();
			serviceCollection.AddSingleton<IRoleRepository<UserAccount>, InMemoryRoleRepository<UserAccount>>();

			serviceCollection.AddMvc()
				.AddApplicationPart(typeof(StartupExtensions).GetTypeInfo().Assembly);

			serviceCollection.AddSilkSignalsServices();

			serviceCollection.Configure<RazorViewEngineOptions>(razorOptions =>
			{
				razorOptions.FileProviders.Add(new EmbeddedFileProvider(Assembly.GetEntryAssembly()));
				razorOptions.FileProviders.Add(new EmbeddedFileProvider(typeof(StartupExtensions).GetTypeInfo().Assembly));
			});

			return WebBuilder;
		}

		public static IApplicationBuilder UseSilkWebCore(this IApplicationBuilder appBuilder)
		{
			if (WebBuilder == null)
				throw new Exception("Must add silk core web services.");

			WebConfig = WebBuilder.Build(appBuilder);
			foreach (var serviceInit in WebConfig.ServiceInitializers)
			{
				appBuilder.ApplicationServices.RunServiceInitializer(serviceInit);
			}

			appBuilder.UseMiddleware<WebCoreMiddleware>();
			appBuilder.UseStaticFiles();
			appBuilder.UseMvc(routes =>
			{
				routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
			});
			return appBuilder;
		}

		private static void RunServiceInitializer(this IServiceProvider serviceProvider, MethodInfo initalizerMethod)
		{
			var parameters = initalizerMethod.GetParameters();
			var arguments = new object[parameters.Length];
			for (var i = 0; i < parameters.Length; i++)
			{
				arguments[i] = serviceProvider.GetService(parameters[i].ParameterType);
			}
			initalizerMethod.Invoke(null, arguments);
		}

		private static IWebContext WebContextFactory(IServiceProvider serviceProvider)
		{
			return serviceProvider.GetRequiredService<WebContextState>().Context;
		}

		private static GlobalViewModel GlobalViewModelFactory(IServiceProvider serviceProvider)
		{
			var viewModel = new GlobalViewModel();

			var webContext = serviceProvider.GetRequiredService<IWebContext>();
			viewModel.Page.SiteName = webContext.Site.SiteName;

			var dashboard = webContext.Dashboard;
			if (dashboard != null)
			{
				viewModel.Dashboard = new DashboardViewModel();
				viewModel.Dashboard.Name = dashboard.Name;
				viewModel.Page.Title = dashboard.Name;
				viewModel.Layout = dashboard.Layout;
			}

			return viewModel;
		}
	}
}
