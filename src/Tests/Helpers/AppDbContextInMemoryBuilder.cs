using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Helpers;

/// <summary>
/// Builder para criação de AppDbContext InMemory em testes.
/// Simplifica a configuração de contextos de banco de dados para testes unitários.
/// </summary>
public class AppDbContextInMemoryBuilder
{
    private string _databaseName = Guid.NewGuid().ToString();

    public AppDbContextInMemoryBuilder ComNomeBanco(string nomeBanco)
    {
        _databaseName = nomeBanco;
        return this;
    }

    public AppDbContext Build()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: _databaseName)
            .Options;
        
        return new AppDbContext(options);
    }

    /// <summary>
    /// Cria uma nova instância do builder com GUID único para o banco.
    /// </summary>
    public static AppDbContextInMemoryBuilder Novo() => new();
}
