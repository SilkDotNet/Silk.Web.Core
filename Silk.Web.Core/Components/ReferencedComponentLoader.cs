using System;

namespace Silk.Web.Core.Components
{
	/// <summary>
	/// Handles loaded a component that is in a referenced assembly.
	/// </summary>
	public abstract class ReferencedComponentLoader : IComponentLoader
	{
		/// <summary>
		/// Gets the component's Type.
		/// </summary>
		public Type ComponentType { get; }

		protected ReferencedComponentLoader(Type componentType)
		{
			ComponentType = componentType;
		}

		public override bool Equals(object obj)
		{
			if (obj is ReferencedComponentLoader refLoader)
			{
				return refLoader.ComponentType == ComponentType;
			}
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return ComponentType.GetHashCode();
		}

		public abstract IComponent CreateComponentInstance();
	}

	/// <summary>
	/// Handles loaded a component that is in a referenced assembly.
	/// </summary>
	public class ReferencedComponentLoader<TComponent> : ReferencedComponentLoader
		where TComponent : IComponent, new()
	{
		public ReferencedComponentLoader() : base(typeof(TComponent))
		{
		}

		public override IComponent CreateComponentInstance()
		{
			return new TComponent();
		}
	}
}
