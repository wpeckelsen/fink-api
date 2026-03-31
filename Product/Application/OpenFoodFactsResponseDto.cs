using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Service.Dtos.OpenFoodFactsDtos;


// full response from the API
public class OpenFoodFactsResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("status_verbose")]
    public string? StatusVerbose { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("product")]
    public OpenFoodFactsResponseProduct? Product { get; set; }

    [JsonPropertyName("blame")]
    public OpenFoodFactsBlame? Blame { get; set; }
}

public class OpenFoodFactsResponseProduct
{
    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("product_name")]
    public string? ProductName { get; set; }

    [JsonPropertyName("brands")]
    public string? Brands { get; set; }

    [JsonPropertyName("categories")]
    public string? Categories { get; set; }

    [JsonPropertyName("quantity")]
    public string? Quantity { get; set; }
}


// ignoring this currently, keeping it around
public class OpenFoodFactsBlame
{
    [JsonPropertyName("fields")]
    public Dictionary<string, OpenFoodFactsBlameEntry>? Fields { get; set; }
}

// ignoring this currently, keeping it around
public class OpenFoodFactsBlameEntry
{
    [JsonPropertyName("value")]
    public JsonElement? Value { get; set; }
}
