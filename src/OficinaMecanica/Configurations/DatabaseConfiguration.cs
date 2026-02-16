using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations
{
    /// <summary>
    /// Configuração do banco de dados
    /// </summary>
    public static class DatabaseConfiguration
    {
        /// <summary>
        /// Configura o Entity Framework Core com PostgreSQL
        /// </summary>
        /// <param name="services">Coleção de serviços</param>
        /// <param name="configuration">Configuração da aplicação</param>
        /// <returns>Coleção de serviços configurada</returns>
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var dbConfig = configuration.GetSection("DatabaseConnection");
            var dbHost = dbConfig["Host"];
            var dbPort = dbConfig["Port"];
            var dbName = dbConfig["DatabaseName"];
            var dbUser = dbConfig["User"];
            var dbPassword = dbConfig["Password"];

            if (string.IsNullOrEmpty(dbHost) || string.IsNullOrEmpty(dbPort) || string.IsNullOrEmpty(dbName) || string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException(
                    $"Configuração de banco de dados incompleta. " +
                    $"Host: {dbHost}, Port: {dbPort}, Database: {dbName}, User: {dbUser}, " +
                    $"Password: {(string.IsNullOrEmpty(dbPassword) ? "VAZIO" : "DEFINIDO")}");
            }

            var connectionString = $"Host={dbHost};Port={dbPort};Database={dbName};Username={dbUser};Password={dbPassword}";
                     
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            Console.WriteLine($"Conectado ao banco: Host={dbHost}, Port={dbPort}, Database={dbName}, User={dbUser}");

            return services;
        }
    }
}
