using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Memory;
using Api.Data;
using Newtonsoft.Json;
using Api;

namespace Api
{
	public class NwsManager(HttpClient httpClient, IMemoryCache cache, IWebHostEnvironment webHostEnvironment)
	{
		private static readonly JsonSerializerOptions options = new(JsonSerializerDefaults.Web);

		public async Task<Zone[]?> GetZonesAsync()
		{
			return await cache.GetOrCreateAsync("zones", async entry =>
			{
				entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);

				// To get the live zone data from NWS, uncomment the following code and comment out the return statement below.
				// This is required if you are deploying to ACA.
				//var zones = await httpClient.GetFromJsonAsync<ZonesResponse>("https://api.weather.gov/zones?type=forecast", options);
				//return zones?.Features
				//            ?.Where(f => f.Properties?.ObservationStations?.Count > 0)
				//            .Select(f => (Zone)f)
				//            .Distinct()
				//            .ToArray() ?? [];

				// Deserialize the zones.json file from the wwwroot folder
				var zonesFilePath = Path.Combine(webHostEnvironment.WebRootPath, "zones.json");
				if (!File.Exists(zonesFilePath))
				{
					return [];
				}

				using var zonesJson = File.OpenRead(zonesFilePath);
				var zones = await System.Text.Json.JsonSerializer.DeserializeAsync<ZonesResponse>(zonesJson, options);

				return zones?.Features
							?.Where(f => f.Properties?.ObservationStations?.Count > 0)
							.Select(f => (Zone)f)
							.Distinct()
							.ToArray() ?? [];
			});
		}

		private static int forecastCount = 0;

		public async Task<Forecast[]> GetForecastByZoneAsync(string zoneId)
		{
			// Create an exception every 5 calls to simulate and error for testing
			forecastCount++;

			if (forecastCount % 5 == 0)
			{
				throw new Exception("Random exception thrown by NwsManager.GetForecastAsync");
			}

			var zoneIdSegment = HttpUtility.UrlEncode(zoneId);
			var zoneUrl = $"https://api.weather.gov/zones/forecast/{zoneIdSegment}/forecast";
			//var forecasts = await httpClient.GetFromJsonAsync<ForecastResponse>(zoneUrl, options);
			string philade = "{\r\n    \"@context\": {\r\n        \"@version\": \"1.1\"\r\n    },\r\n    \"type\": \"Feature\",\r\n    \"geometry\": {\r\n        \"type\": \"Polygon\",\r\n        \"coordinates\": [\r\n            [\r\n                [\r\n                    -75.000999399999998,\r\n                    40.129512699999999\r\n                ],\r\n                [\r\n                    -74.9985961,\r\n                    40.128211899999997\r\n                ],\r\n                [\r\n                    -74.994300800000005,\r\n                    40.131210299999999\r\n                ],\r\n                [\r\n                    -74.992599400000003,\r\n                    40.1296119\r\n                ],\r\n                [\r\n                    -74.992996200000007,\r\n                    40.126411400000002\r\n                ],\r\n                [\r\n                    -74.989692600000012,\r\n                    40.126010800000003\r\n                ],\r\n                [\r\n                    -74.987693700000008,\r\n                    40.123912800000006\r\n                ],\r\n                [\r\n                    -74.984893700000015,\r\n                    40.123912800000006\r\n                ],\r\n                [\r\n                    -74.983093200000013,\r\n                    40.12311170000001\r\n                ],\r\n                [\r\n                    -74.982200600000013,\r\n                    40.120311700000009\r\n                ],\r\n                [\r\n                    -74.978897000000018,\r\n                    40.119613600000008\r\n                ],\r\n                [\r\n                    -75.093399000000062,\r\n                    40.072013800000015\r\n                ],\r\n                [\r\n                    -75.09169760000006,\r\n                    40.073913500000018\r\n                ],\r\n                [\r\n                    -75.089500400000063,\r\n                    40.07581320000002\r\n                ],\r\n                [\r\n                    -75.084396300000066,\r\n                    40.081111900000018\r\n                ],\r\n                [\r\n                    -75.081596300000072,\r\n                    40.083713500000016\r\n                ],\r\n                [\r\n                    -75.078292800000071,\r\n                    40.085311800000014\r\n                ],\r\n                [\r\n                    -75.076095500000065,\r\n                    40.08731070000001\r\n                ],\r\n                [\r\n                    -75.071395800000062,\r\n                    40.092514000000008\r\n                ],\r\n                [\r\n                    -75.065299900000056,\r\n                    40.098812100000011\r\n                ],\r\n                [\r\n                    -75.060997000000057,\r\n                    40.103210400000009\r\n                ],\r\n                [\r\n                    -75.057594200000054,\r\n                    40.106311700000006\r\n                ],\r\n                [\r\n                    -75.04329680000005,\r\n                    40.116111700000005\r\n                ],\r\n                [\r\n                    -75.023292500000053,\r\n                    40.130210800000008\r\n                ],\r\n                [\r\n                    -75.017196600000048,\r\n                    40.135513300000007\r\n                ],\r\n                [\r\n                    -75.015396100000046,\r\n                    40.137413000000009\r\n                ],\r\n                [\r\n                    -75.013595500000051,\r\n                    40.137111600000011\r\n                ],\r\n                [\r\n                    -75.006294200000056,\r\n                    40.133010800000008\r\n                ],\r\n                [\r\n                    -75.000999399999998,\r\n                    40.129512699999999\r\n                ]\r\n            ]\r\n        ]\r\n    },\r\n    \"properties\": {\r\n        \"zone\": \"https://api.weather.gov/zones/forecast/PAZ071\",\r\n        \"updated\": \"2024-10-02T04:00:00-04:00\",\r\n        \"periods\": [\r\n            {\r\n                \"number\": 1,\r\n                \"name\": \"Today\",\r\n                \"detailedForecast\": \"Cloudy. Highs around 70. Northeast winds 5 to 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 2,\r\n                \"name\": \"Tonight\",\r\n                \"detailedForecast\": \"Mostly cloudy. Lows in the upper 50s. Northeast winds around 5 mph.\"\r\n            },\r\n            {\r\n                \"number\": 3,\r\n                \"name\": \"Thursday\",\r\n                \"detailedForecast\": \"Partly sunny. Highs in the mid 70s. Northeast winds around 5 mph, becoming south around 5 mph in the afternoon.\"\r\n            },\r\n            {\r\n                \"number\": 4,\r\n                \"name\": \"Thursday Night\",\r\n                \"detailedForecast\": \"Partly cloudy. Lows in the upper 50s. South winds around 5 mph.\"\r\n            },\r\n            {\r\n                \"number\": 5,\r\n                \"name\": \"Friday\",\r\n                \"detailedForecast\": \"Partly sunny. Highs in the upper 70s. South winds around 5 mph.\"\r\n            },\r\n            {\r\n                \"number\": 6,\r\n                \"name\": \"Friday Night\",\r\n                \"detailedForecast\": \"Mostly cloudy. Lows around 60.\"\r\n            },\r\n            {\r\n                \"number\": 7,\r\n                \"name\": \"Saturday\",\r\n                \"detailedForecast\": \"Partly sunny in the morning, then clearing. Highs in the upper 70s.\"\r\n            },\r\n            {\r\n                \"number\": 8,\r\n                \"name\": \"Saturday Night\",\r\n                \"detailedForecast\": \"Mostly clear. Cooler with lows around 50.\"\r\n            },\r\n            {\r\n                \"number\": 9,\r\n                \"name\": \"Sunday\",\r\n                \"detailedForecast\": \"Sunny. Highs in the mid 70s.\"\r\n            },\r\n            {\r\n                \"number\": 10,\r\n                \"name\": \"Sunday Night\",\r\n                \"detailedForecast\": \"Partly cloudy in the evening, then mostly cloudy with a chance of showers after midnight. Lows in the mid 50s. Chance of rain 30 percent.\"\r\n            },\r\n            {\r\n                \"number\": 11,\r\n                \"name\": \"Monday\",\r\n                \"detailedForecast\": \"Partly sunny with a 30 percent chance of showers. Highs in the lower 70s.\"\r\n            },\r\n            {\r\n                \"number\": 12,\r\n                \"name\": \"Monday Night\",\r\n                \"detailedForecast\": \"Mostly cloudy. A chance of showers in the evening. Lows in the lower 50s. Chance of rain 30 percent.\"\r\n            },\r\n            {\r\n                \"number\": 13,\r\n                \"name\": \"Tuesday\",\r\n                \"detailedForecast\": \"Mostly sunny. Highs in the upper 60s.\"\r\n            }\r\n        ]\r\n    }\r\n}";
			string manhutan = "{\r\n    \"@context\": {\r\n        \"@version\": \"1.1\"\r\n    },\r\n    \"type\": \"Feature\",\r\n    \"geometry\": {\r\n        \"type\": \"MultiPolygon\",\r\n        \"coordinates\": [\r\n            [\r\n                [\r\n                    [\r\n                        -74.044217900000007,\r\n                        40.688613500000002\r\n                    ],\r\n                    [\r\n                        -74.044217900000007,\r\n                        40.688613500000002\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -74.022035000000002,\r\n                        40.683999000000007\r\n                    ],\r\n                    [\r\n                        -74.022035000000002,\r\n                        40.683999000000007\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -74.038995600000021,\r\n                        40.700942400000024\r\n                    ],\r\n                    [\r\n                        -74.037340000000015,\r\n                        40.699504000000026\r\n                    ],\r\n                    [\r\n                        -74.039352399999999,\r\n                        40.701251900000017\r\n                    ],\r\n                    [\r\n                        -74.038995600000021,\r\n                        40.700942400000024\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -73.919494599999993,\r\n                        40.876026100000018\r\n                    ],\r\n                    [\r\n                        -73.917594899999997,\r\n                        40.875324200000016\r\n                    ],\r\n                    [\r\n                        -73.915199200000004,\r\n                        40.875724700000013\r\n                    ],\r\n                    [\r\n                        -73.933402600000079,\r\n                        40.882076799999979\r\n                    ],\r\n                    [\r\n                        -73.924598200000077,\r\n                        40.879010099999981\r\n                    ],\r\n                    [\r\n                        -73.924696800000078,\r\n                        40.87890059999998\r\n                    ],\r\n                    [\r\n                        -73.922096200000084,\r\n                        40.877826599999977\r\n                    ],\r\n                    [\r\n                        -73.919494599999993,\r\n                        40.876026100000018\r\n                    ]\r\n                ]\r\n            ]\r\n        ]\r\n    },\r\n    \"properties\": {\r\n        \"zone\": \"https://api.weather.gov/zones/forecast/NYZ072\",\r\n        \"updated\": \"2024-10-02T05:34:00-04:00\",\r\n        \"periods\": [\r\n            {\r\n                \"number\": 1,\r\n                \"name\": \"Today\",\r\n                \"detailedForecast\": \"Mostly cloudy. Highs in the upper 60s. Northeast winds 5 to 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 2,\r\n                \"name\": \"Tonight\",\r\n                \"detailedForecast\": \"Mostly cloudy in the evening, then becoming partly cloudy. Lows in the upper 50s. East winds around 5 mph.\"\r\n            },\r\n            {\r\n                \"number\": 3,\r\n                \"name\": \"Thursday\",\r\n                \"detailedForecast\": \"Mostly sunny. Highs in the lower 70s. Southeast winds 5 to 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 4,\r\n                \"name\": \"Thursday Night\",\r\n                \"detailedForecast\": \"Partly cloudy. Lows in the upper 50s. South winds 5 to 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 5,\r\n                \"name\": \"Friday\",\r\n                \"detailedForecast\": \"Mostly sunny. Highs in the mid 70s. South winds 5 to 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 6,\r\n                \"name\": \"Friday Night\",\r\n                \"detailedForecast\": \"Mostly cloudy. Lows in the lower 60s.\"\r\n            },\r\n            {\r\n                \"number\": 7,\r\n                \"name\": \"Saturday\",\r\n                \"detailedForecast\": \"Mostly sunny. Highs in the upper 70s.\"\r\n            },\r\n            {\r\n                \"number\": 8,\r\n                \"name\": \"Saturday Night\",\r\n                \"detailedForecast\": \"Mostly clear. Lows in the mid 50s.\"\r\n            },\r\n            {\r\n                \"number\": 9,\r\n                \"name\": \"Sunday\",\r\n                \"detailedForecast\": \"Sunny. Highs in the lower 70s.\"\r\n            },\r\n            {\r\n                \"number\": 10,\r\n                \"name\": \"Sunday Night\",\r\n                \"detailedForecast\": \"Partly cloudy in the evening, then mostly cloudy with a chance of showers after midnight. Lows in the upper 50s. Chance of rain 30 percent.\"\r\n            },\r\n            {\r\n                \"number\": 11,\r\n                \"name\": \"Monday\",\r\n                \"detailedForecast\": \"Partly sunny with a 40 percent chance of showers. Highs in the lower 70s.\"\r\n            },\r\n            {\r\n                \"number\": 12,\r\n                \"name\": \"Monday Night\",\r\n                \"detailedForecast\": \"Partly cloudy with a 40 percent chance of showers. Lows in the lower 50s.\"\r\n            },\r\n            {\r\n                \"number\": 13,\r\n                \"name\": \"Tuesday\",\r\n                \"detailedForecast\": \"Mostly sunny. Highs in the mid 60s.\"\r\n            }\r\n        ]\r\n    }\r\n}";
			string juana = "{\r\n    \"@context\": {\r\n        \"@version\": \"1.1\"\r\n    },\r\n    \"type\": \"Feature\",\r\n    \"geometry\": {\r\n        \"type\": \"MultiPolygon\",\r\n        \"coordinates\": [\r\n            [\r\n                [\r\n                    [\r\n                        -133.6942822,\r\n                        57.796781799999998\r\n                    ],\r\n                    [\r\n                        -133.69353409999999,\r\n                        57.796718299999995\r\n                    ],\r\n                    [\r\n                        -133.6935814,\r\n                        57.796719899999992\r\n                    ],\r\n                    [\r\n                        -133.6942822,\r\n                        57.796781799999998\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.5371610000004,\r\n                        58.350806000000148\r\n                    ],\r\n                    [\r\n                        -134.5371610000004,\r\n                        58.350806000000148\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.61046400000041,\r\n                        58.354234000000147\r\n                    ],\r\n                    [\r\n                        -134.61046400000041,\r\n                        58.354234000000147\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.69726100000042,\r\n                        58.361464000000147\r\n                    ],\r\n                    [\r\n                        -134.69726100000042,\r\n                        58.361464000000147\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.9902550000003,\r\n                        58.463705000000125\r\n                    ],\r\n                    [\r\n                        -134.9902550000003,\r\n                        58.463705000000125\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.91369500000033,\r\n                        58.486343000000147\r\n                    ],\r\n                    [\r\n                        -134.91369500000033,\r\n                        58.486343000000147\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.99638800000011,\r\n                        58.502694000000098\r\n                    ],\r\n                    [\r\n                        -135.02409900000009,\r\n                        58.518662000000084\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -135.03554300000005,\r\n                        58.527967000000089\r\n                    ],\r\n                    [\r\n                        -135.03554300000005,\r\n                        58.527967000000089\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.91575400000011,\r\n                        58.565586000000117\r\n                    ],\r\n                    [\r\n                        -134.91239840000011,\r\n                        58.563588200000119\r\n                    ],\r\n                    [\r\n                        -134.91887700000015,\r\n                        58.568338000000118\r\n                    ],\r\n                    [\r\n                        -134.91575400000011,\r\n                        58.565586000000117\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.95386300000015,\r\n                        58.629698000000118\r\n                    ],\r\n                    [\r\n                        -134.9539000000002,\r\n                        58.632172000000111\r\n                    ],\r\n                    [\r\n                        -134.95334390000019,\r\n                        58.631446000000111\r\n                    ],\r\n                    [\r\n                        -134.95320100000021,\r\n                        58.630553000000113\r\n                    ],\r\n                    [\r\n                        -134.95386300000015,\r\n                        58.629698000000118\r\n                    ]\r\n                ]\r\n            ],\r\n            [\r\n                [\r\n                    [\r\n                        -134.9552180000002,\r\n                        58.674361000000111\r\n                    ],\r\n                    [\r\n                        -134.95560800000021,\r\n                        58.674072000000109\r\n                    ],\r\n                    [\r\n                        -134.95607900000022,\r\n                        58.674174000000107\r\n                    ],\r\n                    [\r\n                        -134.95635600000023,\r\n                        58.67492700000011\r\n                    ],\r\n                    [\r\n                        -134.95588700000022,\r\n                        58.675157000000112\r\n                    ],\r\n                    [\r\n                        -134.9552180000002,\r\n                        58.674361000000111\r\n                    ]\r\n                ]\r\n            ]\r\n        ]\r\n    },\r\n    \"properties\": {\r\n        \"zone\": \"https://api.weather.gov/zones/forecast/AKZ325\",\r\n        \"updated\": \"2024-10-01T20:30:00-08:00\",\r\n        \"periods\": [\r\n            {\r\n                \"number\": 1,\r\n                \"name\": \"Tonight\",\r\n                \"detailedForecast\": \"Mostly cloudy. Lows in the lower 40s. North winds around 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 2,\r\n                \"name\": \"Wednesday\",\r\n                \"detailedForecast\": \"Rain. Highs around 50. Southeast winds around 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 3,\r\n                \"name\": \"Wednesday Night\",\r\n                \"detailedForecast\": \"Rain showers. Lows in the lower 40s. Southeast winds around 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 4,\r\n                \"name\": \"Thursday\",\r\n                \"detailedForecast\": \"Rain. Highs in the upper 40s. East winds around 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 5,\r\n                \"name\": \"Thursday Night\",\r\n                \"detailedForecast\": \"Mostly cloudy. Chance of rain. Lows in the upper 30s. East winds around 10 mph.\"\r\n            },\r\n            {\r\n                \"number\": 6,\r\n                \"name\": \"Friday\",\r\n                \"detailedForecast\": \"Partly cloudy. Chance of rain. Highs 45 to 51.\"\r\n            },\r\n            {\r\n                \"number\": 7,\r\n                \"name\": \"Friday Night\",\r\n                \"detailedForecast\": \"Rain. Lows in the lower 40s.\"\r\n            },\r\n            {\r\n                \"number\": 8,\r\n                \"name\": \"Saturday\",\r\n                \"detailedForecast\": \"Rain. Highs in the upper 40s.\"\r\n            },\r\n            {\r\n                \"number\": 9,\r\n                \"name\": \"Saturday Night\",\r\n                \"detailedForecast\": \"Rain. Lows in the mid 40s.\"\r\n            },\r\n            {\r\n                \"number\": 10,\r\n                \"name\": \"Sunday\",\r\n                \"detailedForecast\": \"Rain. Highs in the upper 40s.\"\r\n            },\r\n            {\r\n                \"number\": 11,\r\n                \"name\": \"Sunday Night\",\r\n                \"detailedForecast\": \"Rain. Lows in the lower 40s.\"\r\n            },\r\n            {\r\n                \"number\": 12,\r\n                \"name\": \"Monday\",\r\n                \"detailedForecast\": \"Rain. Highs in the upper 40s.\"\r\n            },\r\n            {\r\n                \"number\": 13,\r\n                \"name\": \"Monday Night\",\r\n                \"detailedForecast\": \"Rain. Lows in the lower 40s.\"\r\n            },\r\n            {\r\n                \"number\": 14,\r\n                \"name\": \"Tuesday\",\r\n                \"detailedForecast\": \"Rain. Highs 45 to 51.\"\r\n            }\r\n        ]\r\n    }\r\n}";

			ForecastResponse dataResult = null;
			if (zoneIdSegment == "PAZ071")
				dataResult = JsonConvert.DeserializeObject<ForecastResponse>(philade);
			else if (zoneIdSegment == "NYZ072")
				dataResult = JsonConvert.DeserializeObject<ForecastResponse>(manhutan);
			else if (zoneIdSegment == "AKZ325")
				dataResult = JsonConvert.DeserializeObject<ForecastResponse>(juana);
			else
				dataResult = await httpClient.GetFromJsonAsync<ForecastResponse>(zoneUrl, options);



			return dataResult
				   ?.Properties
				   ?.Periods
				   ?.Select(p => (Forecast)p)
				   .ToArray() ?? [];
		}
	}
}

namespace Microsoft.Extensions.DependencyInjection
{
	public static class NwsManagerExtensions
	{
		public static IServiceCollection AddNwsManager(this IServiceCollection services)
		{
			services.AddHttpClient<Api.NwsManager>(client =>
			{
				client.BaseAddress = new Uri("https://api.weather.gov/");
				client.DefaultRequestHeaders.Add("User-Agent", "Microsoft .NET Aspire Demo");
			});

			services.AddMemoryCache();

			// Add default output caching
			services.AddOutputCache(options =>
			{
				options.AddBasePolicy(builder => builder.Cache());
			});

			return services;
		}

		public static WebApplication? MapApiEndpoints(this WebApplication app)
		{
			app.UseOutputCache();

			app.MapGet("/zones", async (Api.NwsManager manager) =>
				{
					var zones = await manager.GetZonesAsync();
					return TypedResults.Ok(zones);
				})
				.CacheOutput(policy => policy.Expire(TimeSpan.FromHours(1)))
				.WithName("GetZones");

			app.MapGet("/forecast/{zoneId}", async Task<Results<Ok<Api.Forecast[]>, NotFound>> (Api.NwsManager manager, string zoneId) =>
				{
					try
					{
						var forecasts = await manager.GetForecastByZoneAsync(zoneId);
						return TypedResults.Ok(forecasts);
					}
					catch (HttpRequestException)
					{
						return TypedResults.NotFound();
					}
				})
				.CacheOutput(policy => policy.Expire(TimeSpan.FromMinutes(15)).SetVaryByRouteValue("zoneId"))
				.WithName("GetForecastByZone")
				.WithOpenApi();

			return app;
		}


	}
}