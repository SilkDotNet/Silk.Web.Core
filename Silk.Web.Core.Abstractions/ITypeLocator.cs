using System;
using System.Collections.Generic;

namespace Silk.Web.Core
{
	/// <summary>
	/// Locates types within the web application.
	/// </summary>
	public interface ITypeLocator
	{
		/// <summary>
		/// Find types that implement a specific interface.
		/// </summary>
		/// <typeparam name="TInterface"></typeparam>
		/// <returns></returns>
		IEnumerable<Type> FindImplementations<TInterface>(bool includeAbstractClasses = false,
			bool includeInterfaces = false);
	}
}
