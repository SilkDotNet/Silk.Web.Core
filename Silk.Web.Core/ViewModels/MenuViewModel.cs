using Silk.Mapping;
using Silk.Web.Core.Abstractions;

namespace Silk.Web.Core.ViewModels
{
	[AutoMap(typeof(IMenu), MapFromTargetType = true, MapToTargetType = false)]
	public class MenuViewModel
	{
		public string MenuName { get; private set; }
		public string MenuTitle { get; private set; }
		public IMenuItem[] MenuItems { get; private set; }
	}
}
