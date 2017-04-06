using System;
using System.Reflection;

namespace Silk.Web.Core.Abstractions
{
	public class Dashboard
	{
		public string Layout { get; }
		public string Name { get; }
		public Type ControllerType { get; }
		public TypeInfo ControllerTypeInfo { get; }
		public string Path { get; }
		public IDashboardAccessChecker AccessChecker { get; }

		public Dashboard(string layout, string name, TypeInfo controllerType, string path,
			IDashboardAccessChecker accessChecker)
		{
			Layout = layout;
			Name = name;
			ControllerTypeInfo = controllerType;
			ControllerType = controllerType.AsType();
			Path = path;
			AccessChecker = accessChecker;
		}
	}
}
