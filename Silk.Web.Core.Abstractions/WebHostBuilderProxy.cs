using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Silk.Web.Core
{
	/// <summary>
	/// Helper class for hanging builders off IWebHostBuilder.
	/// </summary>
	public abstract class WebHostBuilderProxy : IWebHostBuilder
	{
		protected IWebHostBuilder Builder { get; }

		protected WebHostBuilderProxy(IWebHostBuilder builder)
		{
			Builder = builder;
		}

		public IWebHost Build()
		{
			return Builder.Build();
		}

		public IWebHostBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
		{
			return Builder.ConfigureAppConfiguration(configureDelegate);
		}

		public IWebHostBuilder ConfigureServices(Action<IServiceCollection> configureServices)
		{
			return Builder.ConfigureServices(configureServices);
		}

		public IWebHostBuilder ConfigureServices(Action<WebHostBuilderContext, IServiceCollection> configureServices)
		{
			return Builder.ConfigureServices(configureServices);
		}

		public string GetSetting(string key)
		{
			return Builder.GetSetting(key);
		}

		public IWebHostBuilder UseSetting(string key, string value)
		{
			return Builder.UseSetting(key, value);
		}
	}
}
