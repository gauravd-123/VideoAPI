using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using static VideoAPI.Models.WhatsAppMessage;
using VideoAPI.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace VideoAPI.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class WhatsappController : ControllerBase
	{
		private readonly string _verifyToken = "tokenVal";
		private readonly HttpClient _httpClient;
		
		//private static string _whatsappToken = "EAAI09S5xNMwBO9sEDH6hNZAJhuuE94MEc9tjUqugCKDOlQyMG8uIPEZC6KtWew4vwujWiZCttWnaN8ZAuqmrZAmwVdaAIZBtex3CVQPmHDM5ccEE7nKJl09jgmHdIfL5xCj0DB1nxBE6bwu49fRKwkHj3Dq4vGI0KXIPEvmMmK4Q13nRlepKcZBMGx7HebFqFi5xn8Odf2CEHEfHSkQNxxVeztlgKcZD";
		private static string _whatsappToken = "EAAI09S5xNMwBOx5VXl7iJoWh4dxA0vA7xgvaNIhn5YmS2BUVM1ZCa4NcAEZCTmqSY3UBQOSgXmgtkxP7U1e07KXZAZC2MQDo1ovYd2SwIBIkyZAHxcc8y9wrzdKxz9Q3QgpmQjjjyAtzdh6nFjWe4QvF65pQupKe4RJvZCenOb76UEGhu4SGUmHJydZCoXS3ubg1wZDZD";
		private readonly string _phoneNumberId = "624174124105868";

		public WhatsappController(IHttpClientFactory httpClientFactory)
		{
			Console.WriteLine("WhatsappController Constructor Called!");
			_httpClient = httpClientFactory.CreateClient();
		}


		//[HttpGet("webhook")]
		//public IActionResult VerifyWebhook([FromQuery(Name = "hub.mode")] string hubMode,
		//						   [FromQuery(Name = "hub.challenge")] int hubChallenge,
		//						   [FromQuery(Name = "hub.verify_token")] string hubVerifyToken)
		//{
		//	Console.WriteLine($"Received Webhook Verification: mode={hubMode}, challenge={hubChallenge}, token={hubVerifyToken}");

		//	if (hubMode == "subscribe" && hubVerifyToken == _verifyToken)
		//	{
		//		Console.WriteLine("Webhook verified successfully!");
		//		return Content(hubChallenge.ToString(), "text/plain"); // Ensure plain text response
		//	}

		//	Console.WriteLine("Webhook verification failed!");
		//	return Unauthorized();
		//}

		//[HttpPost("webhook")]
		[HttpPost, HttpGet]
		public async Task<IActionResult> ReceiveMessage()
		{
			Console.WriteLine(Request);
			var req = Request;
			try
			{
				string mode = req.Query["hub.mode"];
				string challenge = req.Query["hub.challenge"];
				string verifyToken = req.Query["hub.verify_token"];
				if (!string.IsNullOrEmpty(mode) && !string.IsNullOrEmpty(verifyToken))
				{
					if (mode == "subscribe" && verifyToken == _verifyToken)
					{
						Console.WriteLine("NewToken: " + _whatsappToken);
						Console.WriteLine("Webhook verified successfully!");
						//return Content(hubChallenge.ToString(), "text/plain"); // Ensure plain text response
						//int respMsg = challenge;
						return new OkObjectResult(challenge.ToString());
					}
					else
					{
						Console.WriteLine("Error Validation");
						return new BadRequestObjectResult("Error in hub.mode or verify_token");
					}
				} else
				{
					using var reader = new StreamReader(Request.Body);
					string requestBody = await reader.ReadToEndAsync();

					// Parse the JSON dynamically
					var receivedMessage = JObject.Parse(requestBody);

					// Extract required fields dynamically
					var entry = receivedMessage["entry"]?.First;
					var changes = entry?["changes"]?.First;
					var value = changes?["value"];
					var messages = value?["messages"]?.First;

					if (messages != null)
					{
						


						string senderPhoneNumber = messages["from"]?.ToString();
						string messageType = messages["type"]?.ToString();
						string messageText = messages["text"]?["body"]?.ToString();

						var interactive = messages["interactive"];
						if (interactive?["type"]?.ToString() == "button_reply")
						{
							string buttonId = interactive["button_reply"]?["id"]?.ToString();

							if (buttonId == "generate_quote")
							{
								await SendWhatsAppMessage(senderPhoneNumber, "Fetching perfect plan for your vehicle...");
								await Task.Delay(4000);
								// Call your quote API here
								string quoteDetails = "Total Premium: ₹ 22803";

								await SendWhatsAppMessage(senderPhoneNumber, quoteDetails);
								string appUrl = "https://nysa.icicilombard.com/#/login";
								await SendWhatsAppMessage(senderPhoneNumber, $"Click here to proceed: {appUrl}");
								//await SendWhatsAppButtonMessage(senderPhoneNumber, "goto_Nysa", "Go to Nysa");
							}else if (buttonId == "goto_Nysa")
							{
								//await SendWhatsAppMessage(senderPhoneNumber, "Redirecting to Nysa...");

								
								// Call your quote API here
								//string quoteDetails = "Total Premium: ₹ 22803";

								//await SendWhatsAppMessage(senderPhoneNumber, quoteDetails);
								//await SendWhatsAppButtonMessage(senderPhoneNumber, "goto_Nysa", "Go to Nysa");
							}
						} else
						{
							// Log for debugging
							Console.WriteLine($"Received message from: {senderPhoneNumber}");
							Console.WriteLine($"Message Type: {messageType}");
							Console.WriteLine($"Message Text: {messageText}");

							if (!string.IsNullOrEmpty(messageText))
							{
								string responseMessage = $"Received: {messageText}\nFetching details...";
								await SendWhatsAppMessage(senderPhoneNumber, responseMessage);
								await Task.Delay(4000);

								responseMessage = $"RTO: MAHARASHTRA-MUMBAI\n\nManufacturer: TATA MOTORS\nVehicle sub-class: TRUCKS\nBody type: Closed\nModel build: FULLY BUILT\nModel: TATA 709.\nCubic Capacity: 3784\nGross Vehicle Weight: 6800\nSeating Capacity: 2\nCarrying Capacity: 2\nFuel Type: Diesel G";
								await SendWhatsAppMessage(senderPhoneNumber, responseMessage);
								await SendWhatsAppButtonMessage(senderPhoneNumber, "generate_quote", "Generate Quote");
							}
						}
					}
					return Ok();
				}


			}
			catch (Exception ex)
			{
				return Content("Error occured. StackTrace: " + ex.StackTrace +", Message: "+ex.Message);
			}

			

		}

		

	// Model for receiving token update requests
	

	

		private async Task SendWhatsAppMessage(string to, string message)
		{
			var url = $"https://graph.facebook.com/v17.0/{_phoneNumberId}/messages";

			var payload = new
			{
				messaging_product = "whatsapp",
				to = to,
				type = "text",
				text = new { body = message }
			};

			var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
			_httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", _whatsappToken);

			await _httpClient.PostAsync(url, content);
		}

		private async Task SendWhatsAppButtonMessage(string phoneNumber, string btnName, string btnText)
		{
			var payload = new
			{
				messaging_product = "whatsapp",
				recipient_type = "individual",
				to = phoneNumber,
				type = "interactive",
				interactive = new
				{
					type = "button",
					body = new { text = "Would you like to generate a quote for your vehicle?" },
					action = new
					{
						buttons = new[]
						{
					new { type = "reply", reply = new { id = btnName, title = btnText } }
				}
					}
				}
			};

			await SendWhatsAppPayload(payload);
		}

		private async Task SendWhatsAppPayload(object payload)
		{
			string token = _whatsappToken; // Fetch latest token from DB
			string url = $"https://graph.facebook.com/v19.0/{_phoneNumberId}/messages";

			using var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
			request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			request.Content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");

			using var response = await _httpClient.SendAsync(request);
			string responseBody = await response.Content.ReadAsStringAsync();

			Console.WriteLine($"WhatsApp API Response: {responseBody}");
		}




		[HttpPost("webhook01")]
		public async Task<IActionResult> ReceiveMessage([FromBody] WhatsAppMessage request)
		{
			if (request.Entry?[0]?.Changes?[0]?.Value?.Messages == null)
				return Ok();

			var message = request.Entry[0].Changes[0].Value.Messages[0];

			if (message.Type == "text")
			{
				string userMessage = message.Text.Body;
				string userNumber = message.From;

				// Process user input (Vehicle Registration Number)
				string responseMessage = $"Received: {userMessage}\nFetching details...";

				await SendWhatsAppMessage(userNumber, responseMessage);
			}

			return Ok();
		}
	}


}
