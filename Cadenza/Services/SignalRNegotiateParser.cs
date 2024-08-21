using System.Text.Json.Serialization;

namespace Cadenza.Services
{
	public class SignalRNegotiateParser
	{
		[JsonPropertyName("url")]
		public string Url { get; set; }
		[JsonPropertyName("accessToken")]
		public string AccessToken { get; set; }
	}
}