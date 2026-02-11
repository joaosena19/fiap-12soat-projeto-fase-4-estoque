using Infrastructure.Database;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Tests.Helpers;

namespace Tests.Integration
{
    public class TestWebApplicationFactory<TStartup> : WebApplicationFactory<TStartup>
        where TStartup : class
    {
        private readonly string _databaseName;

        public TestWebApplicationFactory()
        {
            // Gera uma database única para cada classe de teste, útil para múltiplos testes não interferirem no mesmo banco e corromperem outros testes.
            var stackTrace = new StackTrace();
            var callingFrame = stackTrace.GetFrames()
                ?.FirstOrDefault(f => f.GetMethod()?.DeclaringType?.Name?.EndsWith("Tests") == true);
            
            var testClassName = callingFrame?.GetMethod()?.DeclaringType?.Name ?? "Unknown";
            _databaseName = $"TestDb_{testClassName}_{Guid.NewGuid():N}";
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Sobrescreve o appsettings para usar o HmacSecret de teste
            builder.ConfigureAppConfiguration((context, config) =>
            {
                var overrides = new Dictionary<string, string?>
                {
                    ["Jwt:Key"] = JwtTestConstants.Key,
                    ["Jwt:Issuer"] = JwtTestConstants.Issuer,
                    ["Jwt:Audience"] = JwtTestConstants.Audience,
                    ["Logging:LogLevel:Default"] = "None",
                    ["Logging:LogLevel:Microsoft"] = "None",
                    ["Logging:LogLevel:Microsoft.AspNetCore"] = "None",
                    ["Logging:LogLevel:Microsoft.EntityFrameworkCore"] = "None"
                };
                config.AddInMemoryCollection(overrides);
            });

            builder.ConfigureServices(services =>
            {
                // Remove DbContext atual, com conexão com o banco real
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<AppDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Cria conexão com banco de dados em memória
                services.AddDbContext<AppDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_databaseName);
                });

                // Reconfigura JWT com valores de teste
                services.Configure<Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerOptions>(
                    Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme,
                    options =>
                    {
                        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                        {
                            ValidateIssuerSigningKey = true,
                            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(
                                System.Text.Encoding.UTF8.GetBytes(JwtTestConstants.Key)),
                            ValidateIssuer = true,
                            ValidIssuer = JwtTestConstants.Issuer,
                            ValidateAudience = true,
                            ValidAudience = JwtTestConstants.Audience,
                            ValidateLifetime = true,
                            ClockSkew = TimeSpan.Zero
                        };
                    });

                // Configura o ServiceProvider para inicializar o banco
                var serviceProvider = services.BuildServiceProvider();
                using var scope = serviceProvider.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                // Garante que o banco é criado e inicializado com os dados de seed
                context.Database.EnsureCreated();
            });

            base.ConfigureWebHost(builder);
        }
    }
}
