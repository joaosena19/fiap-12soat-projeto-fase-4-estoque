using FluentAssertions;
using Infrastructure.Monitoramento.Correlation;

namespace Tests.Infrastructure.Monitoramento.Correlation;

public class CorrelationContextTests
{
    [Fact]
    public void Current_ShouldReturnNull_WhenNoCorrelationIdIsSet()
    {
        // Arrange & Act
        var current = CorrelationContext.Current;

        // Assert
        current.Should().BeNull();
    }

    [Fact]
    public void Push_ShouldSetCurrentCorrelationId()
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

    [Fact]
    public void Push_ShouldRestorePreviousValue_WhenDisposed()
    {
        // Arrange
        var firstId = "first-id";
        var secondId = "second-id";

        // Act
        using (CorrelationContext.Push(firstId))
        {
            CorrelationContext.Current.Should().Be(firstId);

            using (CorrelationContext.Push(secondId))
            {
                CorrelationContext.Current.Should().Be(secondId);
            }

            // Assert - should restore to firstId after inner scope
            CorrelationContext.Current.Should().Be(firstId);
        }

        // Assert - should restore to null after outer scope
        CorrelationContext.Current.Should().BeNull();
    }

    [Fact]
    public void Push_ShouldSupportNesting()
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

    [Fact]
    public void Push_ShouldHandleEmptyString()
    {
        // Arrange
        var emptyId = "";

        // Act
        using (CorrelationContext.Push(emptyId))
        {
            // Assert
            CorrelationContext.Current.Should().Be(emptyId);
        }

        CorrelationContext.Current.Should().BeNull();
    }

    [Fact]
    public void Push_ShouldBeThreadSafe_WithAsyncLocal()
    {
        // Arrange
        var id1 = "thread-1-id";
        var id2 = "thread-2-id";
        var task1Result = "";
        var task2Result = "";

        // Act
        var task1 = Task.Run(() =>
        {
            using (CorrelationContext.Push(id1))
            {
                Thread.Sleep(50); // Simulate some work
                task1Result = CorrelationContext.Current ?? "";
            }
        });

        var task2 = Task.Run(() =>
        {
            using (CorrelationContext.Push(id2))
            {
                Thread.Sleep(50); // Simulate some work
                task2Result = CorrelationContext.Current ?? "";
            }
        });

        Task.WaitAll(task1, task2);

        // Assert - each task should have seen its own correlation ID
        task1Result.Should().Be(id1);
        task2Result.Should().Be(id2);
        CorrelationContext.Current.Should().BeNull();
    }

    [Fact]
    public void Current_ShouldReturnCorrectValue_InDifferentScopes()
    {
        // Arrange
        var outerScopeId = "outer-scope";
        var innerScopeId = "inner-scope";

        // Act & Assert
        CorrelationContext.Current.Should().BeNull();

        using (var outerScope = CorrelationContext.Push(outerScopeId))
        {
            CorrelationContext.Current.Should().Be(outerScopeId);

            using (var innerScope = CorrelationContext.Push(innerScopeId))
            {
                CorrelationContext.Current.Should().Be(innerScopeId);
            }

            CorrelationContext.Current.Should().Be(outerScopeId);
        }

        CorrelationContext.Current.Should().BeNull();
    }

    [Fact]
    public void Dispose_ShouldBeIdempotent()
    {
        // Arrange
        var correlationId = "test-id";
        var scope = CorrelationContext.Push(correlationId);

        // Act
        scope.Dispose();
        scope.Dispose(); // Second dispose should not throw

        // Assert
        CorrelationContext.Current.Should().BeNull();
    }
}
