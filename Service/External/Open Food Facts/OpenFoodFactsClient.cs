using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Service.External.OpenFoodFacts;

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

    public async Task<OpenFoodFactsProductResponse?> GetProductByBarcodeAsync(
        string barcode,
        string? productType = null,
        string? fields = null,
        bool includeBlame = false)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            throw new ArgumentException("Barcode is required.", nameof(barcode));
        }

        var endpoint = string.Format(OpenFoodFactsEndpoints.ProductByBarcode, barcode);
        var query = BuildQuery(productType, fields, includeBlame);
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
        return await JsonSerializer.DeserializeAsync<OpenFoodFactsProductResponse>(contentStream, SerializerOptions).ConfigureAwait(false);
    }

    private static string BuildQuery(string? productType, string? fields, bool includeBlame)
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
        if (includeBlame)
        {
            Append("blame", "1");
        }

        return builder.ToString();
    }
}
