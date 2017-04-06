using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface IMenuBuilder
	{
		Task<IMenu> GetMenuAsync(string menuName, bool forceRebuild = false);
		IMenu GetMenu(string menuName, bool forceRebuild = false);
	}
}
