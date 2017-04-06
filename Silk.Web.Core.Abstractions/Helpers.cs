namespace Silk.Web.Core.Abstractions
{
	public static class Helpers
	{
		public static string ControllerName(this string str)
		{
			if (str.EndsWith("Controller"))
				return str.Substring(0, str.Length - 10);
			return str;
		}
	}
}
