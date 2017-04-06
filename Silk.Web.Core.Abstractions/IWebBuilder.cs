using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Silk.Web.Core.Abstractions
{
	public interface IWebBuilder
	{
		Site DefaultSite { get; }
		Site[] Sites { get; }

		IWebBuilder AddAuthentication(string cookieName = null);

		IWebBuilder UseSiteRepository<TRepository>(ServiceLifetime serviceLifetime) where TRepository : ISiteRepository;
		IWebBuilder UseAccountRepository<TAccount, TRepository>(ServiceLifetime serviceLifetime)
			where TAccount : IAccount
			where TRepository : IAccountRepository<TAccount>;
		IWebBuilder UseRoleRepository<TAccount, TRepository>(ServiceLifetime serviceLifetime)
			where TAccount : IAccount
			where TRepository : IRoleRepository<TAccount>;

		IWebBuilder SetSite(Site site);
		IWebBuilder AddSite(Site site);
		IWebBuilder AddSuperAdminUser(string loginId = "superadmin", string password = null);

		IWebConfig Build(IApplicationBuilder applicationBuilder);
	}
}
