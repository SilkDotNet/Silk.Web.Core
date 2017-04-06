using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Silk.Web.Core.Abstractions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Collections.Generic;

namespace Silk.Web.Core
{
	internal class WebBuilder : IWebBuilder
	{
		private readonly IServiceCollection _serviceCollection;
		private bool _addAuthenticationToPipeline = false;
		private string _cookieName = null;
		private readonly List<Site> _sites = new List<Site>();
		private UserAccount _superUserAccount;
		private UsernamePasswordCredentials _superUserCredentials;

		public Site DefaultSite => _sites.FirstOrDefault();
		public Site[] Sites => _sites.ToArray();

		public WebBuilder(IServiceCollection serviceCollection)
		{
			_serviceCollection = serviceCollection;
		}

		public IWebBuilder AddAuthentication(string cookieName = null)
		{
			if (_addAuthenticationToPipeline)
				return this;
			_serviceCollection.AddAuthentication();
			_addAuthenticationToPipeline = true;
			_cookieName = cookieName;
			if (string.IsNullOrEmpty(_cookieName))
				_cookieName = "silk.user";
			return this;
		}

		public IWebBuilder UseSiteRepository<TRepository>(ServiceLifetime serviceLifetime) where TRepository : ISiteRepository
		{
			ReplaceService<ISiteRepository, TRepository>(serviceLifetime);
			return this;
		}

		public IWebBuilder UseRoleRepository<TAccount, TRepository>(ServiceLifetime serviceLifetime)
			where TAccount : IAccount
			where TRepository : IRoleRepository<TAccount>
		{
			ReplaceService<IRoleRepository<TAccount>, TRepository>(serviceLifetime);
			return this;
		}

		public IWebConfig Build(IApplicationBuilder applicationBuilder)
		{
			using (var scope = applicationBuilder.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				if (_addAuthenticationToPipeline)
				{
					applicationBuilder.UseCookieAuthentication(new CookieAuthenticationOptions
					{
						AutomaticAuthenticate = true,
						AutomaticChallenge = true,
						AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
						LoginPath = new PathString("/signin"),
						AccessDeniedPath = new PathString("/signin"),
						CookieName = _cookieName
					});
				}

				if (_superUserAccount != null)
				{
					var accountRepository = scope.ServiceProvider.GetRequiredService<IAccountRepository<UserAccount>>();
					accountRepository.CreateAccount(_superUserAccount);
					accountRepository.SetAccountCredentials(_superUserAccount, _superUserCredentials);

					var roleRepository = scope.ServiceProvider.GetRequiredService<IRoleRepository<UserAccount>>();
					roleRepository.GrantRoles(_superUserAccount, "SuperAdmin");
				}
			}
			return new WebConfig(this, _serviceCollection);
		}

		public IWebBuilder UseAccountRepository<TAccount, TRepository>(ServiceLifetime serviceLifetime)
			where TAccount : IAccount
			where TRepository : IAccountRepository<TAccount>
		{
			ReplaceService<IAccountRepository<TAccount>, TRepository>(serviceLifetime);
			return this;
		}

		public IWebBuilder SetSite(Site site)
		{
			_sites.Clear();
			_sites.Add(site);
			return this;
		}

		public IWebBuilder AddSite(Site site)
		{
			_sites.Add(site);
			return this;
		}

		public IWebBuilder AddSuperAdminUser(string loginId = "superadmin", string password = null)
		{
			_superUserAccount = new UserAccount
			{
				DisplayName = loginId,
				Slug = loginId
			};
			_superUserCredentials = new UsernamePasswordCredentials(loginId, password);
			return this;
		}

		private void ReplaceService<TService, TNewImplementation>(ServiceLifetime serviceLifetime) where TNewImplementation : TService
		{
			var toReplace = _serviceCollection.FirstOrDefault(q => q.ServiceType == typeof(TService));
			var replaceIndex = -1;
			if (toReplace != null)
			{
				replaceIndex = _serviceCollection.IndexOf(toReplace);
				_serviceCollection.RemoveAt(replaceIndex);
			}
			var serviceDescriptor = new ServiceDescriptor(typeof(TService), typeof(TNewImplementation), serviceLifetime);
			if (replaceIndex != -1)
				_serviceCollection.Insert(replaceIndex, serviceDescriptor);
			else
				_serviceCollection.Add(serviceDescriptor);
		}
	}
}
