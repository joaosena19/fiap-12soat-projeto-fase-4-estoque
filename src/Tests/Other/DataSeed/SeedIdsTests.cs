using FluentAssertions;
using Shared.Seed;
using System.Reflection;

namespace Tests.Other.DataSeed
{
    public class SeedIdsTests
    {
        [Fact(DisplayName = "SeedIds deve conter GUIDs esperados quando definidos")]
        [Trait("Componente", "Seed")]
        public void SeedIds_DeveConterGuidsEsperados_QuandoDefinidos()
        {
            // Arrange & Act & Assert - Veiculos
            SeedIds.Veiculos.Abc1234.Should().Be(Guid.Parse("3f8a2d3b-0d8b-4a3f-9b1e-7b65e6d2a901"));
            SeedIds.Veiculos.Xyz5678.Should().Be(Guid.Parse("0d2c5f44-6a50-4f8e-8d7a-0d6c7b0d1b2c"));
            SeedIds.Veiculos.Def9012.Should().Be(Guid.Parse("9b6d2a10-6a2f-4f7a-9e1b-2a3f0d8b3f8a"));

            // Arrange & Act & Assert - Servicos
            SeedIds.Servicos.TrocaDeOleo.Should().Be(Guid.Parse("1a111111-1111-1111-1111-111111111111"));
            SeedIds.Servicos.AlinhamentoBalanceamento.Should().Be(Guid.Parse("2b222222-2222-2222-2222-222222222222"));
            SeedIds.Servicos.RevisaoCompleta.Should().Be(Guid.Parse("3c333333-3333-3333-3333-333333333333"));

            // Arrange & Act & Assert - ItensEstoque
            SeedIds.ItensEstoque.OleoMotor5w30.Should().Be(Guid.Parse("4d444444-4444-4444-4444-444444444444"));
            SeedIds.ItensEstoque.FiltroDeOleo.Should().Be(Guid.Parse("5e555555-5555-5555-5555-555555555555"));
            SeedIds.ItensEstoque.PastilhaDeFreioDianteira.Should().Be(Guid.Parse("6f666666-6666-6666-6666-666666666666"));
        }

        [Fact(DisplayName = "SeedIds não deve conter GUIDs duplicados quando comparar todas constantes")]
        [Trait("Componente", "Seed")]
        public void SeedIds_NaoDeveConterGuidsDuplicados_QuandoCompararTodasConstantes()
        {
            // Arrange
            var todosGuids = new List<Guid>();
            var tipoSeedIds = typeof(SeedIds);
            var classeAninhadas = tipoSeedIds.GetNestedTypes(BindingFlags.Public | BindingFlags.Static);

            // Act
            foreach (var classeAninhada in classeAninhadas)
            {
                var campos = classeAninhada.GetFields(BindingFlags.Public | BindingFlags.Static);
                foreach (var campo in campos)
                    if (campo.FieldType == typeof(Guid))
                        todosGuids.Add((Guid)campo.GetValue(null)!);
            }

            var guidsUnicos = todosGuids.Distinct().ToList();

            // Assert
            guidsUnicos.Should().HaveCount(todosGuids.Count, "não deve haver GUIDs duplicados entre as constantes de seed");
        }
    }
}
