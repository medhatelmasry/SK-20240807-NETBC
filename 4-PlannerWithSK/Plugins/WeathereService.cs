using System;
using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace PlannerWithSK.Plugins;

public class WeatherService
{

    [KernelFunction, Description("Get the weather details about a city")]
    public static async Task<string> GetWeatherAsync([Description("The city for which the weather is needed")] string city)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        string _apiKey = config["WEATHER_API_KEY"]!;

        string baseUrl = "http://api.openweathermap.org/data/2.5/weather";
        string requestUri = $"{baseUrl}?q={city}&appid={_apiKey}";

        using (HttpClient client = new HttpClient())
        {
            HttpResponseMessage response = await client.GetAsync(requestUri);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                dynamic weatherData = JsonConvert.DeserializeObject(json)!;
                return $"Current weather in {city}: {weatherData.weather[0].description}";
            }
            else
            {
                return $"Error: Unable to retrieve weather data for {city}";
            }
        }
    }
}
