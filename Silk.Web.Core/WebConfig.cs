using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Silk.Data.Schemas;
using Silk.Web.Core.Abstractions;
using System.Collections.Generic;
using System.Linq;
using Silk.Signals;

namespace Silk.Web.Core
{
	internal class WebConfig : IWebConfig
	{
		public TableSchema[] DatabaseTables { get; private set; }
		public MethodInfo[] ServiceInitializers { get; }

		public WebConfig(WebBuilder webBuilder, IServiceCollection serviceCollection)
		{
			ScanServicesForTableSchemas(serviceCollection);
			ServiceInitializers = ScanServicesForInitializers(serviceCollection);
		}

		private MethodInfo[] ScanServicesForInitializers(IServiceCollection serviceCollection)
		{
			var ret = new List<MethodInfo>();

			foreach (var serviceDefinition in serviceCollection)
			{
				if (serviceDefinition.ImplementationType == null)
					continue;

				var serviceType = serviceDefinition.ImplementationType;
				var serviceTypeInfo = serviceType.GetTypeInfo();

				if (serviceTypeInfo.IsGenericType && serviceTypeInfo.GetGenericTypeDefinition() == typeof(OriginalService<,>))
				{
					serviceType = serviceTypeInfo.GetGenericArguments()[1];
					serviceTypeInfo = serviceType.GetTypeInfo();
				}

				foreach (var method in serviceType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public))
				{
					var serviceInitAttr = method.GetCustomAttribute<ServiceInitializeAttribute>();
					if (serviceInitAttr == null)
						continue;
					ret.Add(method);
				}
			}

			return ret.ToArray();
		}

		private void ScanServicesForTableSchemas(IServiceCollection serviceCollection)
		{
			var dbTableList = new List<TableSchema>();
			foreach(var serviceDefinition in serviceCollection)
			{
				if (serviceDefinition.ImplementationType == null)
					continue;

				var serviceType = serviceDefinition.ImplementationType;
				var serviceTypeInfo = serviceType.GetTypeInfo();

				if (serviceTypeInfo.IsGenericType && serviceTypeInfo.GetGenericTypeDefinition() == typeof(OriginalService<,>))
				{
					serviceType = serviceTypeInfo.GetGenericArguments()[1];
					serviceTypeInfo = serviceType.GetTypeInfo();
				}

				var tableAttributes = serviceTypeInfo.GetCustomAttributes<DbTableAttribute>();
				foreach (var tableAttr in tableAttributes)
					dbTableList.Add(TableSchema.FromPoco(tableAttr.SchemaType));

				foreach (var property in serviceType.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(q => q.CanRead))
				{
					var generatedTableAttribute = property.GetCustomAttribute<GeneratedDbTableAttribute>();
					if (generatedTableAttribute == null)
						continue;

					var invokeResult = property.GetMethod.Invoke(null, new object[0]);
					if (invokeResult is TableSchema schema)
						dbTableList.Add(schema);
					else if (invokeResult is TableSchema[] schemas)
						dbTableList.AddRange(schemas);
				}

				foreach (var method in serviceType.GetMethods(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public)
					.Where(q => q.GetParameters().Length == 0))
				{
					var generatedTableAttribute = method.GetCustomAttribute<GeneratedDbTableAttribute>();
					if (generatedTableAttribute == null)
						continue;

					var invokeResult = method.Invoke(null, new object[0]);
					if (invokeResult is TableSchema schema)
						dbTableList.Add(schema);
					else if (invokeResult is TableSchema[] schemas)
						dbTableList.AddRange(schemas);
				}
			}
			DatabaseTables = dbTableList.ToArray();
		}
	}
}
