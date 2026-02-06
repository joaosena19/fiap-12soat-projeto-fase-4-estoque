using System.Text.Json.Serialization;

namespace API.Configurations
{
    /// <summary>
    /// Configuração de controllers
    /// </summary>
    public static class ControllersConfiguration
    {
        /// <summary>
        /// Configura os controllers com autorização obrigatória e conversão de enums
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddApiControllers(this IServiceCollection services)
        {
            services.AddControllers(options =>
                {
                    // Requer autorização para todos os controllers por padrão
                    options.Filters.Add(new Microsoft.AspNetCore.Mvc.Authorization.AuthorizeFilter());
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                });

            return services;
        }
    }
}
