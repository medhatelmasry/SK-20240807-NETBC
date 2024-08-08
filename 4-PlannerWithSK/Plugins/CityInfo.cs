using System;
using System.ComponentModel;
using Microsoft.SemanticKernel;
using Newtonsoft.Json;

namespace PlannerWithSK.Plugins;

public class CityInfo {
    public string? City { get; set; }

    [KernelFunction, Description("Get the City Name from the given JSON string")]
    public static string GetCityNameFromJson([Description("The JSON string from which City Name needs to be extracted")] string jsonString)
    {
        try {
            if (!string.IsNullOrEmpty(jsonString)) {
                CityInfo? cityInfo = JsonConvert.DeserializeObject<CityInfo>(jsonString);
                if (cityInfo != null && cityInfo.City != "Unknown") {
                    return cityInfo.City!;
                } else {
                    return "Invalid or unknown city.";
                }
            } else {
                return "Invalid or unknown city.";
            }
        } catch (JsonException) {
            return "JSON parsing error";
        }
    }
}
