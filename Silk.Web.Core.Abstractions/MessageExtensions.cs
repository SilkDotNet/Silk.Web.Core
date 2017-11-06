using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Silk.Web.Core
{
	public static class MessagesExtensions
	{
		private const string _separator = "˅";

		private static void AddMessage(ITempDataDictionary tempData, string key, string message)
		{
			if (tempData.ContainsKey(key))
			{
				tempData[key] = tempData[key] + _separator + message;
			}
			else
			{
				tempData[key] = message;
			}
		}

		private static string[] GetMessages(ITempDataDictionary tempData, string key)
		{
			if (!tempData.ContainsKey(key))
				return new string[0];
			return ((string)tempData[key])
				.Split(new[] { _separator }, System.StringSplitOptions.RemoveEmptyEntries);
		}

		public static void AddErrorMessage(this ITempDataDictionary tempData, string message)
		{
			AddMessage(tempData, "messages:error", message);
		}

		public static string[] GetErrorMessages(this ITempDataDictionary tempData)
		{
			return GetMessages(tempData, "messages:error");
		}

		public static void AddInfoMessage(this ITempDataDictionary tempData, string message)
		{
			AddMessage(tempData, "messages:info", message);
		}

		public static string[] GetInfoMessages(this ITempDataDictionary tempData)
		{
			return GetMessages(tempData, "messages:info");
		}

		public static void AddSuccessMessage(this ITempDataDictionary tempData, string message)
		{
			AddMessage(tempData, "messages:success", message);
		}

		public static string[] GetSuccessMessages(this ITempDataDictionary tempData)
		{
			return GetMessages(tempData, "messages:success");
		}
	}
}
