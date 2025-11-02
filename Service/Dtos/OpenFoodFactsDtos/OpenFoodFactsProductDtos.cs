using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Service.External.OpenFoodFacts;

public class OpenFoodFactsProductResponse
{
    [JsonPropertyName("status")]
    public int Status { get; set; }

    [JsonPropertyName("status_verbose")]
    public string? StatusVerbose { get; set; }

    [JsonPropertyName("code")]
    public string? Code { get; set; }

    [JsonPropertyName("product")]
    public OpenFoodFactsProduct? Product { get; set; }

    [JsonPropertyName("blame")]
    public OpenFoodFactsBlame? Blame { get; set; }
}

public class OpenFoodFactsBlame
{
    [JsonPropertyName("fields")]
    public Dictionary<string, OpenFoodFactsBlameEntry>? Fields { get; set; }

    [JsonPropertyName("nutriments")]
    public Dictionary<string, OpenFoodFactsBlameEntry>? Nutriments { get; set; }

    [JsonPropertyName("uploaded_images")]
    public Dictionary<string, OpenFoodFactsBlameEntry>? UploadedImages { get; set; }

    [JsonPropertyName("selected_images")]
    public Dictionary<string, OpenFoodFactsBlameEntry>? SelectedImages { get; set; }
}

public class OpenFoodFactsBlameEntry
{
    [JsonPropertyName("userid")]
    public string? UserId { get; set; }

    [JsonPropertyName("t")]
    public long? Timestamp { get; set; }

    [JsonPropertyName("rev")]
    public int? Revision { get; set; }

    [JsonPropertyName("value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? Value { get; set; }

    [JsonPropertyName("previous_userid")]
    public string? PreviousUserId { get; set; }

    [JsonPropertyName("previous_t")]
    public long? PreviousTimestamp { get; set; }

    [JsonPropertyName("previous_rev")]
    public int? PreviousRevision { get; set; }

    [JsonPropertyName("previous_value")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public JsonElement? PreviousValue { get; set; }
}
