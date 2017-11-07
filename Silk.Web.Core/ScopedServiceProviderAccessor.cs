using Microsoft.AspNetCore.Http;
using System;
using System.Threading;

namespace Silk.Web.Core
{
	public class ScopedServiceProviderAccessor : IScopedServiceProviderAccessor
	{
		private static AsyncLocal<IServiceProvider> _scopedServiceProvider = new AsyncLocal<IServiceProvider>();

		private readonly IHttpContextAccessor _httpContextAccessor;

		internal IServiceProvider CurrentScopedServiceProvider
		{
			get
			{
				return _scopedServiceProvider.Value;
			}
			set
			{
				_scopedServiceProvider.Value = value;
			}
		}

		public ScopedServiceProviderAccessor(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public IServiceProvider ScopedServiceProvider
		{
			get
			{
				if (_httpContextAccessor.HttpContext != null &&
					_httpContextAccessor.HttpContext.RequestServices != null)
					return _httpContextAccessor.HttpContext.RequestServices;
				return CurrentScopedServiceProvider;
			}
		}
	}
}
