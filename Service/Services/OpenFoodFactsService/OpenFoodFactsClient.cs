using System;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Service.Dtos.OpenFoodFactsDtos;

namespace Service.Services.OpenFoodFactsService;

public class OpenFoodFactsClient : IOpenFoodFactsClient
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _httpClient;
    private readonly OpenFoodFactsOptions _options;
    private readonly ILogger<OpenFoodFactsClient> _logger;

    public OpenFoodFactsClient(HttpClient httpClient, IOptions<OpenFoodFactsOptions> options, ILogger<OpenFoodFactsClient> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        if (!string.IsNullOrWhiteSpace(_options.BaseUrl))
        {
            _httpClient.BaseAddress = new Uri(_options.BaseUrl, UriKind.Absolute);
        }

        if (!string.IsNullOrWhiteSpace(_options.UserAgent))
        {
            _httpClient.DefaultRequestHeaders.UserAgent.Clear();
            _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd(_options.UserAgent);
        }
    }

    private static readonly Regex QuantityRegex = new(@"^(?<value>\d+(?:[.,]\d+)?)\s*(?<unit>[a-zA-Z]+)$", RegexOptions.Compiled);

    public async Task<OpenFoodFactsDto?> GetProductByBarcodeAsync(
        string barcode,
        string? productType = null,
        string? fields = null)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new ArgumentException("Barcode is required.", nameof(barcode));
        }

        var endpoint = string.Format(OpenFoodFactsEndpoints.ProductByBarcode, barcode);
        var query = BuildQuery(productType, fields);
        endpoint += query;

        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

        if (!string.IsNullOrWhiteSpace(_options.Username) && !string.IsNullOrWhiteSpace(_options.Password))
        {
            var credentials = Encoding.ASCII.GetBytes($"{_options.Username}:{_options.Password}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
        }

        _logger.LogDebug("Calling OpenFoodFacts for barcode {Barcode} at {RequestUri}", barcode, request.RequestUri);

        using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning("OpenFoodFacts request for barcode {Barcode} failed with status {StatusCode}", barcode, (int)response.StatusCode);
            return null;
        }

        await using var contentStream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
        var responseObject = await JsonSerializer.DeserializeAsync<OpenFoodFactsResponse>(contentStream, SerializerOptions).ConfigureAwait(false);
        if (responseObject == null)
        {
            return null;
        }

        var productDto = MapToDto(responseObject);

        return productDto;
    }

    private static string BuildQuery(string? productType, string? fields)
    {
        var builder = new StringBuilder();
        var hasValue = false;

        void Append(string key, string? value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            builder.Append(hasValue ? '&' : '?');
            builder.Append(Uri.EscapeDataString(key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
            hasValue = true;
        }

        Append("product_type", productType);
        Append("fields", fields);
        return builder.ToString();
    }

    private static OpenFoodFactsDto? MapToDto(OpenFoodFactsResponse response)
    {
        if (response.Product == null)
        {
            return null;
        }

        var product = response.Product;
        var dto = new OpenFoodFactsDto
        {
            Barcode = product.Code ?? string.Empty,
            Name = product.ProductName ?? string.Empty,
            Brand = product.Brands ?? string.Empty
        };

        var quantityText = product.Quantity;
        if (!string.IsNullOrWhiteSpace(quantityText) &&
            TryParseQuantity(quantityText!, out var quantity, out var unit))
        {
            dto.Quantity = quantity;
            dto.Unit = unit;
        }

        return dto;
    }

    private static bool TryParseQuantity(string raw, out double quantity, out UnitType unit)
    {
        quantity = default;
        unit = default;

        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        var match = QuantityRegex.Match(raw.Trim());
        if (!match.Success)
        {
            return false;
        }

        var valueText = match.Groups["value"].Value.Replace(',', '.');
        if (!double.TryParse(valueText, NumberStyles.Float, CultureInfo.InvariantCulture, out var numericValue))
        {
            return false;
        }

        var unitToken = match.Groups["unit"].Value.ToLowerInvariant();

        switch (unitToken)
        {
            case "ml":
                quantity = numericValue / 1000d;
                unit = UnitType.Liter;
                return true;
            case "cl":
                quantity = numericValue / 100d;
                unit = UnitType.Liter;
                return true;
            case "dl":
                quantity = numericValue / 10d;
                unit = UnitType.Liter;
                return true;
            case "l":
            case "lt":
                quantity = numericValue;
                unit = UnitType.Liter;
                return true;
            case "g":
                quantity = numericValue / 1000d;
                unit = UnitType.Kilogram;
                return true;
            case "kg":
                quantity = numericValue;
                unit = UnitType.Kilogram;
                return true;
            case "mg":
                quantity = numericValue / 1_000_000d;
                unit = UnitType.Kilogram;
                return true;
            case "pcs":
            case "pc":
            case "piece":
            case "pieces":
            case "st":
                quantity = numericValue;
                unit = UnitType.Piece;
                return true;
            default:
                return false;
        }
    }

}
