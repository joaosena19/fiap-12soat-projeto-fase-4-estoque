using Microsoft.OpenApi.Models;
using System.Reflection;

namespace API.Configurations.Swagger
{
    /// <summary>
    /// Configuração do Swagger/OpenAPI
    /// </summary>
    public static class SwaggerConfiguration
    {
        /// <summary>
        /// Configura os serviços do Swagger para a aplicação
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddSwaggerDocumentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "Oficina Mecânica API", 
                    Version = "v1",
                    Description = "API para gerenciamento de oficina mecânica",
                    Contact = new OpenApiContact
                    {
                        Name = "João Dainese",
                        Email = "joaosenadainese@gmail.com"
                    }
                });

                // Incluir comentários XML
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);

                // Configurar autenticação JWT para o Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });

                // Adiciona filtro personalizado para documentar endpoint de autenticação externa
                c.DocumentFilter<ExternalAuthRouteFilter>();
            });

            return services;
        }

        /// <summary>
        /// Configura o pipeline de middleware do Swagger (apenas em desenvolvimento)
        /// </summary>
        /// <param name="app">Aplicação web</param>
        /// <returns>Aplicação configurada</returns>
        public static WebApplication UseSwaggerDocumentation(this WebApplication app)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Oficina Mecânica API v1");
                    c.RoutePrefix = string.Empty; // Torna o Swagger UI disponível na raiz da aplicação
                });
            }

            return app;
        }
    }
}
