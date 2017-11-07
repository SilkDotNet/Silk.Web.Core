using System;

namespace Silk.Web.Core
{
	public interface IScopedServiceProviderAccessor
	{
		IServiceProvider ScopedServiceProvider { get; }
	}
}
