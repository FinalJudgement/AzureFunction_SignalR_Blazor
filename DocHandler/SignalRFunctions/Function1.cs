using System.Net;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Functions.Worker.Extensions.SignalRService;
using System.Threading;

namespace DocHandler.SignalRFunctions;

public class Functions
{
    private static readonly HttpClient HttpClient = new();
    

	[Function("negotiate")]
	public static async Task<HttpResponseData> Negotiate(
		[HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequestData req,
		[SignalRConnectionInfoInput(HubName = "serverless")] string connectionInfo,
		FunctionContext executionContext)
	{
		var response = req.CreateResponse(HttpStatusCode.OK);
		// Use WriteStringAsync for asynchronous operation
		await response.WriteStringAsync(connectionInfo);
		return response;
	}

    [Function(nameof(SendMessage))]
    [SignalROutput(HubName = "serverless", ConnectionStringSetting = "AzureSignalRConnectionString")]
    public static async Task<SignalRMessageAction> SendMessage(
       [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
    {
        using var bodyReader = new StreamReader(req.Body);
        var messageContent = await bodyReader.ReadToEndAsync(); // Use async method

        return new SignalRMessageAction("newMessage")
        {
            Arguments = new[] { messageContent },
        };
    }
}