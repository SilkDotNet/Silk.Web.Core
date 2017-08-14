using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Silk.Web.Core
{
	public class TypeLocator : ITypeLocator
	{
		private readonly List<Assembly> _assemblyList;

		public TypeLocator(IEnumerable<Assembly> assemblies)
		{
			_assemblyList = new List<Assembly>(assemblies);
		}

		public IEnumerable<Type> FindImplementations<TInterface>(bool includeAbstractClasses = false, bool includeInterfaces = false)
		{
			var interfaceType = typeof(TInterface);
			foreach (var assembly in _assemblyList)
			{
				foreach (var type in assembly.GetTypes())
				{
					if (type.IsAbstract && !includeAbstractClasses)
						continue;
					if (type.IsInterface && !includeInterfaces)
						continue;
					if (type.GetInterfaces().Contains(interfaceType))
						yield return type;
				}
			}
		}
	}
}
