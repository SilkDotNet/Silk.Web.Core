using System;

namespace Silk.Web.Core.Abstractions
{
	public interface IAccount
	{
		Guid AccountId { get; }
		string DisplayName { get; set; }
		string Slug { get; set; }
	}
}
