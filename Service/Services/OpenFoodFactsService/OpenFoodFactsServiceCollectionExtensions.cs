using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Service.Services.OpenFoodFactsService;

public static class OpenFoodFactsServiceCollectionExtensions
{
    public static IServiceCollection AddOpenFoodFacts(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<OpenFoodFactsOptions>(configuration.GetSection(OpenFoodFactsOptions.SectionName));

        services.AddHttpClient<IOpenFoodFactsClient, OpenFoodFactsClient>();

        return services;
    }
}
