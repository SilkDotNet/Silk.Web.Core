namespace Silk.Web.Core.Email
{
	public class EmailOptions
	{
		public string ServerAddress { get; set; }
		public int ServerPort { get; set; }
		public bool UseSSL { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public EmailAddress FromAddress { get; set; }

		public class EmailAddress
		{
			public string Name { get; set; }
			public string Address { get; set; }
		}
	}
}
