using System;

namespace Silk.Web.Core.Persistence
{
	public abstract class Criteria<T>
		where T : Criteria<T>
	{
		private T _criteria;

		protected Criteria()
		{
			_criteria = (T)this;
		}

		public T With(Action<T> criteriaSetter)
		{
			criteriaSetter(_criteria);
			return _criteria;
		}
	}
}
