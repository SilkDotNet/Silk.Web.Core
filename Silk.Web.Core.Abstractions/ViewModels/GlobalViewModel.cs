using System.Collections.Generic;

namespace Silk.Web.Core.Abstractions.ViewModels
{
	public class GlobalViewModel
	{
		public string Layout { get; set; } = "_Layout";
		public DashboardViewModel Dashboard { get; set; }
		public PageViewModel Page { get; set; }
		public List<MessageViewModel> Messages { get; } = new List<MessageViewModel>();

		public GlobalViewModel()
		{
			Page = new PageViewModel();
		}
	}
}
