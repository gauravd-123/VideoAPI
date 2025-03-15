namespace VideoAPI.Models
{
	public class WhatsAppMessage
	{
		public string Object { get; set; }
		public List<Entry> Entry { get; set; }
	}

	public class Entry
	{
		public string Id { get; set; }
		public long Time { get; set; }
		public List<Change> Changes { get; set; }
	}

	public class Change
	{
		public string Field { get; set; }
		public Value Value { get; set; }
	}

	public class Value
	{
		public string MessagingProduct { get; set; }
		public Metadata Metadata { get; set; }
		public List<Contact> Contacts { get; set; }
		public List<Message> Messages { get; set; }
	}

	public class Metadata
	{
		public string DisplayPhoneNumber { get; set; }
		public string PhoneNumberId { get; set; }
	}

	public class Contact
	{
		public Profile Profile { get; set; }
		public string WaId { get; set; }
	}

	public class Profile
	{
		public string Name { get; set; }
	}

	public class Message
	{
		public string From { get; set; }
		public string Id { get; set; }
		public long Timestamp { get; set; }
		public string Type { get; set; }
		public Text Text { get; set; }
	}

	public class Text
	{
		public string Body { get; set; }
	}

	/// <summary>
	/// /////////////////////
	/// </summary>
	public class TokenRequest
	{
		public string Token { get; set; }
	}


}
