namespace Service.External.OpenFoodFacts;

public class OpenFoodFactsOptions
{
    public const string SectionName = "OpenFoodFacts";

    public string BaseUrl { get; set; } = "https://world.openfoodfacts.org";

    public string? Username { get; set; }

    public string? Password { get; set; }

    public string UserAgent { get; set; } = string.Empty;
}
