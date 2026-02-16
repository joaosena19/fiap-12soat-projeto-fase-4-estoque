using Microsoft.Extensions.Logging;
using Moq;

namespace Tests.Infrastructure.SharedHelpers
{
    public static class LoggerMockExtensions
    {
        public static void DeveTerLogado<T>(this Mock<ILogger<T>> mock, LogLevel logLevel) {
            mock.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce,
                $"Era esperado que o método Log fosse chamado pelo menos uma vez com nível {logLevel}.");
        }

        public static void DeveTerLogadoInformation<T>(this Mock<ILogger<T>> mock) {
            mock.DeveTerLogado(LogLevel.Information);
        }

        public static void DeveTerLogadoWarning<T>(this Mock<ILogger<T>> mock) {
            mock.DeveTerLogado(LogLevel.Warning);
        }

        public static void DeveTerLogadoError<T>(this Mock<ILogger<T>> mock) {
            mock.DeveTerLogado(LogLevel.Error);
        }

        public static void NaoDeveTerLogado<T>(this Mock<ILogger<T>> mock, LogLevel logLevel) {
            mock.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never,
                $"O método Log não deveria ter sido chamado com nível {logLevel}.");
        }

        // Overload para ILogger não genérico
        public static void DeveTerLogado(this Mock<ILogger> mock, LogLevel logLevel) {
            mock.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.AtLeastOnce,
                $"Era esperado que o método Log fosse chamado pelo menos uma vez com nível {logLevel}.");
        }

        public static void DeveTerLogadoInformation(this Mock<ILogger> mock) {
            mock.DeveTerLogado(LogLevel.Information);
        }

        public static void DeveTerLogadoWarning(this Mock<ILogger> mock) {
            mock.DeveTerLogado(LogLevel.Warning);
        }

        public static void DeveTerLogadoError(this Mock<ILogger> mock) {
            mock.DeveTerLogado(LogLevel.Error);
        }

        public static void NaoDeveTerLogado(this Mock<ILogger> mock, LogLevel logLevel) {
            mock.Verify(
                x => x.Log(
                    logLevel,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => true),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Never,
                $"O método Log não deveria ter sido chamado com nível {logLevel}.");
        }
    }
}
