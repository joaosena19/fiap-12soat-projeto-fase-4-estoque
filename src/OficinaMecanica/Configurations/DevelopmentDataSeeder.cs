using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace API.Configurations
{
    public static class DevelopmentDataSeeder
    {
        /// <summary>
        /// Popula o banco de dados com dados mock se estiver em ambiente de desenvolvimento e não executando testes de integração
        /// </summary>
        /// <param name="app">A aplicação web</param>
        public static void SeedIfDevelopment(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
                return;

            Seed(app);
        }

        /// <summary>
        /// Popula o banco de dados com dados mock e não executando testes de integração
        /// </summary>
        /// <param name="app">A aplicação web</param>
        public static void Seed(WebApplication app)
        {
            try
            {
                if (IsIntegrationTest())
                    return;

                using var scope = app.Services.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Garante que o banco está criado e as migrations aplicadas
                dbContext.Database.Migrate();

                SeedData.SeedAll(dbContext);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Falha ao popular dados de desenvolvimento", ex);
            }
        }

        private static bool IsIntegrationTest()
        {
            // Verifica o assembly
            return AppDomain.CurrentDomain.GetAssemblies().Any(a => a.FullName?.ToUpper().Contains("TESTS") ?? false);
        }
    }
}
