using System.Threading.Tasks;

namespace Silk.Web.Core.Abstractions
{
	public interface IMenuProvider
	{
		void BuildMenu(IMenu menu);
		Task BuildMenuAsync(IMenu menu);
	}
}
