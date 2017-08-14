using Silk.DataModelling;
using System.Collections.Generic;

namespace Silk.Web.Core.Data
{
	public class FlatFileModelAssembler : IFlatFileModelAssembler
	{
		private readonly List<IViewModellingConvention> _conventions = new List<IViewModellingConvention>
		{
			new CopySimpleTypesConvention(),
			new ReferenceComplexTypesByIdConvention(),
			new AssignEverthingConvention()
		};

		public void ConfigureBuilder(ViewBuilder viewBuilder)
		{
			foreach (var convention in _conventions)
				viewBuilder.AddConvention(convention);
		}
	}
}
