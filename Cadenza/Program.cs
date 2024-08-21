using Cadenza;
using Cadenza.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Net.Http.Json; // For easy JSON deserialization
using System.Text.Json;
using System.Threading.Tasks;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<MessageService>(); // Register the service

await using var host = builder.Build();

// Retrieve HttpClient from the service provider
var httpClient = host.Services.GetRequiredService<HttpClient>();

var baseUrl = builder.Configuration["AzureFunction:BaseUrl"];
Console.WriteLine(baseUrl);

var negotiationUrl = $"http://localhost:7104/api/negotiate"; // Assuming /negotiate is your endpoint
Console.WriteLine(negotiationUrl);

try
{
	// Using GetStringAsync to fetch the URL and then parsing the AccessToken
	string jsonResponse = await httpClient.GetStringAsync(negotiationUrl);
	Console.WriteLine(jsonResponse);
	var connectionInfo = JsonSerializer.Deserialize<SignalRNegotiateParser>(jsonResponse);

	// Check if the deserialization was successful
	if (connectionInfo != null)
	{
		var connection = new HubConnectionBuilder()
			.WithUrl(connectionInfo.Url, options =>
			{
				options.AccessTokenProvider = () => Task.FromResult(connectionInfo.AccessToken);
			})
			.WithAutomaticReconnect()
			.Build();

		builder.Services.AddScoped<HubConnection>(sp => connection);
	}
	else
	{
		Console.WriteLine("Failed to deserialize the connection info.");
	}
}
catch (Exception ex)
{
	Console.WriteLine($"An error occurred: {ex.Message}");
}

await builder.Build().RunAsync();
