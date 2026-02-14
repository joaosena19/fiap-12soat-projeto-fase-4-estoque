using Application.Contracts.Monitoramento;
using FluentAssertions;
using Infrastructure.Monitoramento;
using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento;

public class CorrelationIdAccessorTests
{
    [Fact(DisplayName = "GetCorrelationId deve retornar valor atual quando CorrelationContext estiver definido")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveRetornarValorAtual_QuandoCorrelationContextEstiverDefinido()
    {
        // Arrange
        var correlationIdEsperado = "correlation-x";
        var accessor = new CorrelationIdAccessor();

        // Act
        string resultado;
        using (CorrelationContext.Push(correlationIdEsperado))
            resultado = accessor.GetCorrelationId();

        // Assert
        resultado.Should().Be(correlationIdEsperado);
    }

    [Fact(DisplayName = "GetCorrelationId deve gerar GUID válido quando CorrelationContext estiver nulo ou whitespace")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveGerarGuidValido_QuandoCorrelationContextEstiverNuloOuWhitespace()
    {
        // Arrange
        var accessor = new CorrelationIdAccessor();

        // Act - sem Push, CorrelationContext.Current será null
        var resultado = accessor.GetCorrelationId();

        // Assert
        resultado.Should().NotBeNullOrWhiteSpace();
        var parseResult = Guid.TryParse(resultado, out var guid);
        parseResult.Should().BeTrue();
        guid.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "GetCorrelationId deve gerar GUID válido quando CorrelationContext for string vazia")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveGerarGuidValido_QuandoCorrelationContextForStringVazia()
    {
        // Arrange
        var accessor = new CorrelationIdAccessor();

        // Act
        string resultado;
        using (CorrelationContext.Push(""))
            resultado = accessor.GetCorrelationId();

        // Assert
        resultado.Should().NotBeNullOrWhiteSpace();
        var parseResult = Guid.TryParse(resultado, out var guid);
        parseResult.Should().BeTrue();
        guid.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "GetCorrelationId deve gerar GUID válido quando CorrelationContext for whitespace")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveGerarGuidValido_QuandoCorrelationContextForWhitespace()
    {
        // Arrange
        var accessor = new CorrelationIdAccessor();

        // Act
        string resultado;
        using (CorrelationContext.Push("   "))
            resultado = accessor.GetCorrelationId();

        // Assert
        resultado.Should().NotBeNullOrWhiteSpace();
        var parseResult = Guid.TryParse(resultado, out var guid);
        parseResult.Should().BeTrue();
        guid.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "GetCorrelationId deve retornar valores diferentes quando chamado múltiplas vezes sem contexto")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveRetornarValoresDiferentes_QuandoChamadoMultiplasVezesSemContexto()
    {
        // Arrange
        var accessor = new CorrelationIdAccessor();

        // Act
        var resultado1 = accessor.GetCorrelationId();
        var resultado2 = accessor.GetCorrelationId();
        var resultado3 = accessor.GetCorrelationId();

        // Assert
        resultado1.Should().NotBe(resultado2);
        resultado2.Should().NotBe(resultado3);
        resultado1.Should().NotBe(resultado3);
    }

    [Fact(DisplayName = "GetCorrelationId deve retornar o mesmo valor quando chamado múltiplas vezes dentro do mesmo contexto")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveRetornarMesmoValor_QuandoChamadoMultiplasVezesDentroDoMesmoContexto()
    {
        // Arrange
        var correlationIdEsperado = "test-correlation-123";
        var accessor = new CorrelationIdAccessor();

        // Act & Assert
        using (CorrelationContext.Push(correlationIdEsperado))
        {
            var resultado1 = accessor.GetCorrelationId();
            var resultado2 = accessor.GetCorrelationId();
            var resultado3 = accessor.GetCorrelationId();

            resultado1.Should().Be(correlationIdEsperado);
            resultado2.Should().Be(correlationIdEsperado);
            resultado3.Should().Be(correlationIdEsperado);
        }
    }

    [Fact(DisplayName = "GetCorrelationId deve gerar novo GUID após sair do escopo do contexto")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveGerarNovoGuid_AposSairDoEscopoDoContexto()
    {
        // Arrange
        var correlationIdContexto = "scoped-correlation";
        var accessor = new CorrelationIdAccessor();

        // Act
        string resultadoDentroEscopo;
        using (CorrelationContext.Push(correlationIdContexto))
            resultadoDentroEscopo = accessor.GetCorrelationId();

        var resultadoForaEscopo = accessor.GetCorrelationId();

        // Assert
        resultadoDentroEscopo.Should().Be(correlationIdContexto);
        resultadoForaEscopo.Should().NotBe(correlationIdContexto);
        var parseResult = Guid.TryParse(resultadoForaEscopo, out var guid);
        parseResult.Should().BeTrue();
        guid.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "GetCorrelationId deve respeitar escopos aninhados")]
    [Trait("Infrastructure", "Monitoramento")]
    public void GetCorrelationId_DeveRespeitarEscoposAninhados()
    {
        // Arrange
        var correlationIdExterno = "outer-correlation";
        var correlationIdInterno = "inner-correlation";
        var accessor = new CorrelationIdAccessor();

        // Act & Assert
        using (CorrelationContext.Push(correlationIdExterno))
        {
            var resultadoExterno1 = accessor.GetCorrelationId();
            resultadoExterno1.Should().Be(correlationIdExterno);

            using (CorrelationContext.Push(correlationIdInterno))
            {
                var resultadoInterno = accessor.GetCorrelationId();
                resultadoInterno.Should().Be(correlationIdInterno);
            }

            var resultadoExterno2 = accessor.GetCorrelationId();
            resultadoExterno2.Should().Be(correlationIdExterno);
        }
    }
}
