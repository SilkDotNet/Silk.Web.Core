namespace Silk.Web.Core.Abstractions
{
	/// <summary>
	/// Simple class for grouping permissions and roles into something more palatable for end users.
	/// </summary>
	public class OrganizationGroup
	{
		public string Name { get; }
		public string Description { get; }

		public OrganizationGroup(string name, string description)
		{
			Name = name;
			Description = description;
		}
	}
}
