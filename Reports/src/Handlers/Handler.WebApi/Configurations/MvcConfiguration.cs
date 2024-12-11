using System.Text.Json;
using System.Text.Json.Serialization;
// using Handler.WebApi.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Handler.WebApi.Configurations;

public static class MvcConfiguration
{
    public static void ConfigureMvc(this IServiceCollection services)
    {
        services
            .AddControllers(options =>
            {
                // options.Filters.Add<ValidationFilter>();
                options.Filters.Add(new ProducesAttribute("application/json"));
            })
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
            });
    }
}