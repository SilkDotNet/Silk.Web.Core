using System;

namespace Silk.Web.Core.Abstractions
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
	public class DbTableAttribute : Attribute
	{
		public DbTableAttribute(Type schemaType)
		{
			SchemaType = schemaType;
		}

		public Type SchemaType { get; private set; }
	}
}
