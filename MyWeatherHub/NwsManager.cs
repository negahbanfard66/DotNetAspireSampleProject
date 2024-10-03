using System.Text.Json;
using System.Web;

namespace MyWeatherHub;

public class NwsManager(HttpClient client)
{
	private static readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

	public async Task<Zone[]> GetZonesAsync()
	{
		var response =  await client.GetAsync("zones");
		response.EnsureSuccessStatusCode();

		var content = await response.Content.ReadAsStringAsync();
		var zones = JsonSerializer.Deserialize<Zone[]>(content, options);

		return zones ?? [];
	}

	public async Task<Forecast[]> GetForecastByZoneAsync(string zoneId)
	{
		var response = await client.GetAsync($"forecast/{zoneId}");
		response.EnsureSuccessStatusCode();
		var content = await response.Content.ReadAsStringAsync();
		var forecast = JsonSerializer.Deserialize<Forecast[]>(content, options);
		return forecast ?? [];
	}
}

public record Zone(string Key, string Name, string State);

public record Forecast(string Name, string DetailedForecast);