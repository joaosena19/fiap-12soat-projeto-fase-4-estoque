using FluentAssertions;
using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento.Correlation;

public class CorrelationContextTests
{
    [Fact(DisplayName = "Current deve retornar null quando nenhum CorrelationId está definido")]
    public void Current_DeveRetornarNull_QuandoNenhumCorrelationIdEstaDefinido()
    {
        // Arrange & Act
        var valorAtual = CorrelationContext.Current;

        // Assert
        valorAtual.Should().BeNull();
    }

    [Fact(DisplayName = "Push deve definir o CorrelationId atual")]
    public void Push_DeveDefinirCorrelationIdAtual()
    {
        // Arrange
        var correlationId = "test-correlation-id-123";

        // Act
        using (CorrelationContext.Push(correlationId))
        {
            // Assert
            CorrelationContext.Current.Should().Be(correlationId);
        }
    }

    [Fact(DisplayName = "Push deve restaurar o valor anterior quando descartado")]
    public void Push_DeveRestaurarValorAnterior_QuandoDescartado()
    {
        // Arrange
        var primeiroId = "first-id";
        var segundoId = "second-id";

        // Act
        using (CorrelationContext.Push(primeiroId))
        {
            CorrelationContext.Current.Should().Be(primeiroId);

            using (CorrelationContext.Push(segundoId))
            {
                CorrelationContext.Current.Should().Be(segundoId);
            }

            // Assert - deve restaurar para primeiroId após o escopo interno
            CorrelationContext.Current.Should().Be(primeiroId);
        }

        // Assert - deve restaurar para null após o escopo externo
        CorrelationContext.Current.Should().BeNull();
    }

    [Fact(DisplayName = "Push deve suportar aninhamento")]
    public void Push_DeveSuportarAninhamento()
    {
        // Arrange
        var id1 = "id-1";
        var id2 = "id-2";
        var id3 = "id-3";

        // Act & Assert
        CorrelationContext.Current.Should().BeNull();

        using (CorrelationContext.Push(id1))
        {
            CorrelationContext.Current.Should().Be(id1);

            using (CorrelationContext.Push(id2))
            {
                CorrelationContext.Current.Should().Be(id2);

                using (CorrelationContext.Push(id3))
                {
                    CorrelationContext.Current.Should().Be(id3);
                }

                CorrelationContext.Current.Should().Be(id2);
            }

            CorrelationContext.Current.Should().Be(id1);
        }

        CorrelationContext.Current.Should().BeNull();
    }

    [Fact(DisplayName = "Push deve lidar com string vazia")]
    public void Push_DeveLidarComStringVazia()
    {
        // Arrange
        var idVazio = "";

        // Act
        using (CorrelationContext.Push(idVazio))
        {
            // Assert
            CorrelationContext.Current.Should().Be(idVazio);
        }

        CorrelationContext.Current.Should().BeNull();
    }

    [Fact(DisplayName = "Push deve ser thread-safe com AsyncLocal")]
    public void Push_DeveSerThreadSafe_ComAsyncLocal()
    {
        // Arrange
        var id1 = "thread-1-id";
        var id2 = "thread-2-id";
        var resultadoTask1 = "";
        var resultadoTask2 = "";

        // Act
        var task1 = Task.Run(() =>
        {
            using (CorrelationContext.Push(id1))
            {
                Thread.Sleep(50); // Simula algum trabalho
                resultadoTask1 = CorrelationContext.Current ?? "";
            }
        });

        var task2 = Task.Run(() =>
        {
            using (CorrelationContext.Push(id2))
            {
                Thread.Sleep(50); // Simula algum trabalho
                resultadoTask2 = CorrelationContext.Current ?? "";
            }
        });

        Task.WaitAll(task1, task2);

        // Assert - cada task deve ter visto seu próprio correlation ID
        resultadoTask1.Should().Be(id1);
        resultadoTask2.Should().Be(id2);
        CorrelationContext.Current.Should().BeNull();
    }

    [Fact(DisplayName = "Current deve retornar o valor correto em escopos diferentes")]
    public void Current_DeveRetornarValorCorreto_EmEscoposDiferentes()
    {
        // Arrange
        var idEscopoExterno = "outer-scope";
        var idEscopoInterno = "inner-scope";

        // Act & Assert
        CorrelationContext.Current.Should().BeNull();

        using (var escopoExterno = CorrelationContext.Push(idEscopoExterno))
        {
            CorrelationContext.Current.Should().Be(idEscopoExterno);

            using (var escopoInterno = CorrelationContext.Push(idEscopoInterno))
            {
                CorrelationContext.Current.Should().Be(idEscopoInterno);
            }

            CorrelationContext.Current.Should().Be(idEscopoExterno);
        }

        CorrelationContext.Current.Should().BeNull();
    }

    [Fact(DisplayName = "Dispose deve ser idempotente")]
    public void Dispose_DeveSerIdempotente()
    {
        // Arrange
        var correlationId = "test-id";
        var escopo = CorrelationContext.Push(correlationId);

        // Act
        escopo.Dispose();
        escopo.Dispose(); // Segundo dispose não deve lançar exceção

        // Assert
        CorrelationContext.Current.Should().BeNull();
    }
}
