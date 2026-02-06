using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Domain.OrdemServico.Aggregates.OrdemServico;
using Domain.OrdemServico.Enums;
using Domain.Identidade.Aggregates;
using Domain.Identidade.Enums;
using FluentAssertions;
using Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Tests.Other.DataSeed
{
    public class SeedDataTests : IDisposable
    {
        private readonly AppDbContext _context;

        public SeedDataTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        #region SeedClientes Tests

        [Fact(DisplayName = "Deve popular clientes quando banco estiver vazio")]
        [Trait("Método", "SeedClientes")]
        public void SeedClientes_Deve_PopularClientes_Quando_BancoVazio()
        {
            // Arrange
            _context.Clientes.Should().BeEmpty();

            // Act
            SeedData.SeedClientes(_context);

            // Assert
            var clientes = _context.Clientes.ToList();
            clientes.Should().HaveCount(6);
            clientes.Should().Contain(c => c.Nome.Valor == "João Silva" && c.DocumentoIdentificador.Valor == "56229071010");
            clientes.Should().Contain(c => c.Nome.Valor == "Maria Santos" && c.DocumentoIdentificador.Valor == "99754534063");
            clientes.Should().Contain(c => c.Nome.Valor == "Pedro Oliveira" && c.DocumentoIdentificador.Valor == "13763122044");
            clientes.Should().Contain(c => c.Nome.Valor == "Transportadora Logística Express Ltda" && c.DocumentoIdentificador.Valor == "62255092000108");
            clientes.Should().Contain(c => c.Nome.Valor == "Auto Peças e Serviços São Paulo S.A." && c.DocumentoIdentificador.Valor == "13179173000160");
            clientes.Should().Contain(c => c.Nome.Valor == "cliente" && c.DocumentoIdentificador.Valor == "19649323007");
        }

        [Fact(DisplayName = "Não deve popular clientes quando já existirem dados")]
        [Trait("Método", "SeedClientes")]
        public void SeedClientes_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            var clienteExistente = Cliente.Criar("Cliente Existente", "56229071010");
            _context.Clientes.Add(clienteExistente);
            _context.SaveChanges();

            var quantidadeInicial = _context.Clientes.Count();

            // Act
            SeedData.SeedClientes(_context);

            // Assert
            var quantidadeFinal = _context.Clientes.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
            _context.Clientes.Should().Contain(c => c.Nome.Valor == "Cliente Existente");
        }

        #endregion

        #region SeedVeiculos Tests

        [Fact(DisplayName = "Deve popular veículos quando banco estiver vazio e houver clientes")]
        [Trait("Método", "SeedVeiculos")]
        public void SeedVeiculos_Deve_PopularVeiculos_Quando_BancoVazioEHouverClientes()
        {
            // Arrange
            SeedData.SeedClientes(_context);
            _context.Veiculos.Should().BeEmpty();

            // Act
            SeedData.SeedVeiculos(_context);

            // Assert
            var veiculos = _context.Veiculos.ToList();
            veiculos.Should().HaveCount(5);
            veiculos.Should().Contain(v => v.Placa.Valor == "ABC1234" && v.Modelo.Valor == "Civic" && v.TipoVeiculo.Valor == TipoVeiculoEnum.Carro);
            veiculos.Should().Contain(v => v.Placa.Valor == "XYZ5678" && v.Modelo.Valor == "Corolla" && v.TipoVeiculo.Valor == TipoVeiculoEnum.Carro);
            veiculos.Should().Contain(v => v.Placa.Valor == "DEF9012" && v.Modelo.Valor == "CB 600F" && v.TipoVeiculo.Valor == TipoVeiculoEnum.Moto);
            veiculos.Should().Contain(v => v.Placa.Valor == "GHI3456" && v.Modelo.Valor == "Onix" && v.TipoVeiculo.Valor == TipoVeiculoEnum.Carro);
            veiculos.Should().Contain(v => v.Placa.Valor == "JKL7890" && v.Modelo.Valor == "YZF-R3" && v.TipoVeiculo.Valor == TipoVeiculoEnum.Moto);
        }

        [Fact(DisplayName = "Não deve popular veículos quando já existirem dados")]
        [Trait("Método", "SeedVeiculos")]
        public void SeedVeiculos_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            SeedData.SeedClientes(_context);
            var cliente = _context.Clientes.First();
            var veiculoExistente = Veiculo.Criar(cliente.Id, "XXX1111", "Modelo Teste", "Marca Teste", "Azul", 2020, TipoVeiculoEnum.Carro);
            _context.Veiculos.Add(veiculoExistente);
            _context.SaveChanges();

            var quantidadeInicial = _context.Veiculos.Count();

            // Act
            SeedData.SeedVeiculos(_context);

            // Assert
            var quantidadeFinal = _context.Veiculos.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
            _context.Veiculos.Should().Contain(v => v.Placa.Valor == "XXX1111");
        }

        [Fact(DisplayName = "Não deve popular veículos quando não houver clientes suficientes")]
        [Trait("Método", "SeedVeiculos")]
        public void SeedVeiculos_NaoDevePopular_Quando_NaoHouverClientesSuficientes()
        {
            // Arrange - Criando apenas 2 clientes (menos que os 5 necessários)
            var cliente1 = Cliente.Criar("Cliente 1", "56229071010");
            var cliente2 = Cliente.Criar("Cliente 2", "99754534063");
            _context.Clientes.AddRange(cliente1, cliente2);
            _context.SaveChanges();

            // Act
            SeedData.SeedVeiculos(_context);

            // Assert
            _context.Veiculos.Should().BeEmpty();
        }

        #endregion

        #region SeedServicos Tests

        [Fact(DisplayName = "Deve popular serviços quando banco estiver vazio")]
        [Trait("Método", "SeedServicos")]
        public void SeedServicos_Deve_PopularServicos_Quando_BancoVazio()
        {
            // Arrange
            _context.Servicos.Should().BeEmpty();

            // Act
            SeedData.SeedServicos(_context);

            // Assert
            var servicos = _context.Servicos.ToList();
            servicos.Should().HaveCount(8);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Óleo" && s.Preco.Valor == 80.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Alinhamento e Balanceamento" && s.Preco.Valor == 120.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Revisão Completa" && s.Preco.Valor == 350.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Pastilhas de Freio" && s.Preco.Valor == 180.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Filtro de Ar" && s.Preco.Valor == 45.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Diagnóstico Eletrônico" && s.Preco.Valor == 100.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Troca de Correia Dentada" && s.Preco.Valor == 280.00m);
            servicos.Should().Contain(s => s.Nome.Valor == "Limpeza de Bicos Injetores" && s.Preco.Valor == 150.00m);
        }

        [Fact(DisplayName = "Não deve popular serviços quando já existirem dados")]
        [Trait("Método", "SeedServicos")]
        public void SeedServicos_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            var servicoExistente = Servico.Criar("Serviço Existente", 50.00m);
            _context.Servicos.Add(servicoExistente);
            _context.SaveChanges();

            var quantidadeInicial = _context.Servicos.Count();

            // Act
            SeedData.SeedServicos(_context);

            // Assert
            var quantidadeFinal = _context.Servicos.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
            _context.Servicos.Should().Contain(s => s.Nome.Valor == "Serviço Existente");
        }

        #endregion

        #region SeedItensEstoque Tests

        [Fact(DisplayName = "Deve popular itens de estoque quando banco estiver vazio")]
        [Trait("Método", "SeedItensEstoque")]
        public void SeedItensEstoque_Deve_PopularItens_Quando_BancoVazio()
        {
            // Arrange
            _context.ItensEstoque.Should().BeEmpty();

            // Act
            SeedData.SeedItensEstoque(_context);

            // Assert
            var itens = _context.ItensEstoque.ToList();
            itens.Should().HaveCount(13);

            // Verificar algumas peças
            itens.Should().Contain(i => i.Nome.Valor == "Óleo Motor 5W30" && i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Peca && i.Preco.Valor == 45.90m);
            itens.Should().Contain(i => i.Nome.Valor == "Filtro de Óleo" && i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Peca && i.Preco.Valor == 25.50m);
            itens.Should().Contain(i => i.Nome.Valor == "Pastilha de Freio Dianteira" && i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Peca && i.Preco.Valor == 89.90m);

            // Verificar alguns insumos
            itens.Should().Contain(i => i.Nome.Valor == "Fluido de Freio" && i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Insumo && i.Preco.Valor == 15.90m);
            itens.Should().Contain(i => i.Nome.Valor == "Aditivo para Radiador" && i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Insumo && i.Preco.Valor == 22.50m);
            itens.Should().Contain(i => i.Nome.Valor == "Graxa Multiuso" && i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Insumo && i.Preco.Valor == 8.90m);

            // Verificar tipos
            var pecas = itens.Where(i => i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Peca).ToList();
            var insumos = itens.Where(i => i.TipoItemEstoque.Valor == TipoItemEstoqueEnum.Insumo).ToList();
            pecas.Should().HaveCount(8);
            insumos.Should().HaveCount(5);
        }

        [Fact(DisplayName = "Não deve popular itens de estoque quando já existirem dados")]
        [Trait("Método", "SeedItensEstoque")]
        public void SeedItensEstoque_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            var itemExistente = ItemEstoque.Criar("Item Existente", 10, TipoItemEstoqueEnum.Peca, 25.00m);
            _context.ItensEstoque.Add(itemExistente);
            _context.SaveChanges();

            var quantidadeInicial = _context.ItensEstoque.Count();

            // Act
            SeedData.SeedItensEstoque(_context);

            // Assert
            var quantidadeFinal = _context.ItensEstoque.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
            _context.ItensEstoque.Should().Contain(i => i.Nome.Valor == "Item Existente");
        }

        #endregion

        #region SeedOrdensServico Tests

        [Fact(DisplayName = "Deve popular ordens de serviço quando banco estiver vazio e houver dados necessários")]
        [Trait("Método", "SeedOrdensServico")]
        public void SeedOrdensServico_Deve_PopularOrdens_Quando_BancoVazioEHouverDados()
        {
            // Arrange
            SeedData.SeedClientes(_context);
            SeedData.SeedVeiculos(_context);
            SeedData.SeedServicos(_context);
            SeedData.SeedItensEstoque(_context);
            _context.OrdensServico.Should().BeEmpty();

            // Act
            SeedData.SeedOrdensServico(_context);

            // Assert
            var ordens = _context.OrdensServico.Include(o => o.ItensIncluidos).Include(o => o.ServicosIncluidos).ToList();
            ordens.Should().HaveCount(3);

            // Verificar ordem cancelada
            var osCancelada = ordens.FirstOrDefault(o => o.Status.Valor == StatusOrdemServicoEnum.Cancelada);
            osCancelada.Should().NotBeNull();
            osCancelada!.ItensIncluidos.Should().HaveCount(2);

            // Verificar ordem entregue
            var osEntregue = ordens.FirstOrDefault(o => o.Status.Valor == StatusOrdemServicoEnum.Entregue);
            osEntregue.Should().NotBeNull();
            osEntregue!.ServicosIncluidos.Should().HaveCount(2);

            // Verificar ordem em diagnóstico
            var osEmDiagnostico = ordens.FirstOrDefault(o => o.Status.Valor == StatusOrdemServicoEnum.EmDiagnostico);
            osEmDiagnostico.Should().NotBeNull();
        }

        [Fact(DisplayName = "Não deve popular ordens de serviço quando já existirem dados")]
        [Trait("Método", "SeedOrdensServico")]
        public void SeedOrdensServico_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            SeedData.SeedClientes(_context);
            SeedData.SeedVeiculos(_context);
            SeedData.SeedServicos(_context);
            SeedData.SeedItensEstoque(_context);

            var veiculo = _context.Veiculos.First();
            var osExistente = OrdemServico.Criar(veiculo.Id);
            _context.OrdensServico.Add(osExistente);
            _context.SaveChanges();

            var quantidadeInicial = _context.OrdensServico.Count();

            // Act
            SeedData.SeedOrdensServico(_context);

            // Assert
            var quantidadeFinal = _context.OrdensServico.Count();
            quantidadeFinal.Should().Be(quantidadeInicial);
        }

        [Fact(DisplayName = "Não deve popular ordens de serviço quando não houver dados suficientes")]
        [Trait("Método", "SeedOrdensServico")]
        public void SeedOrdensServico_NaoDevePopular_Quando_NaoHouverDadosSuficientes()
        {
            // Arrange - Criando apenas alguns clientes, sem veículos, serviços ou itens
            var cliente = Cliente.Criar("Cliente Teste", "99754534063");
            _context.Clientes.Add(cliente);
            _context.SaveChanges();

            // Act
            SeedData.SeedOrdensServico(_context);

            // Assert
            _context.OrdensServico.Should().BeEmpty();
        }

        #endregion

        #region SeedUsuarios Tests

        [Fact(DisplayName = "Deve popular usuários quando banco estiver vazio")]
        [Trait("Método", "SeedUsuarios")]
        public void SeedUsuarios_Deve_PopularUsuarios_Quando_BancoVazio()
        {
            // Arrange
            _context.Usuarios.Should().BeEmpty();

            // Act
            SeedData.SeedUsuarios(_context);

            // Assert
            var usuarios = _context.Usuarios.Include(u => u.Roles).ToList();
            usuarios.Should().HaveCount(2);

            // Verificar administrador
            var admin = usuarios.FirstOrDefault(u => u.DocumentoIdentificadorUsuario.Valor == "82954150009");
            admin.Should().NotBeNull();
            admin!.Roles.Should().HaveCount(1);
            admin.Roles.First().Id.Should().Be(RoleEnum.Administrador);

            // Verificar cliente
            var cliente = usuarios.FirstOrDefault(u => u.DocumentoIdentificadorUsuario.Valor == "19649323007");
            cliente.Should().NotBeNull();
            cliente!.Roles.Should().HaveCount(1);
            cliente.Roles.First().Id.Should().Be(RoleEnum.Cliente);
        }

        [Fact(DisplayName = "Não deve popular usuários quando já existirem dados")]
        [Trait("Método", "SeedUsuarios")]
        public void SeedUsuarios_NaoDevePopular_Quando_JaExistiremDados()
        {
            // Arrange
            SeedData.SeedUsuarios(_context);
            var usuariosIniciais = _context.Usuarios.Count();

            // Act
            SeedData.SeedUsuarios(_context);

            // Assert
            _context.Usuarios.Should().HaveCount(usuariosIniciais);
        }

        #endregion

        #region SeedAll Tests

        [Fact(DisplayName = "Deve popular todos os dados quando banco estiver vazio")]
        [Trait("Método", "SeedAll")]
        public void SeedAll_Deve_PopularTodosDados_Quando_BancoVazio()
        {
            // Arrange
            _context.Usuarios.Should().BeEmpty();
            _context.Clientes.Should().BeEmpty();
            _context.Veiculos.Should().BeEmpty();
            _context.Servicos.Should().BeEmpty();
            _context.ItensEstoque.Should().BeEmpty();
            _context.OrdensServico.Should().BeEmpty();

            // Act
            SeedData.SeedAll(_context);

            // Assert
            _context.Usuarios.Should().HaveCount(2);
            _context.Clientes.Count().Should().BeGreaterThanOrEqualTo(6);
            _context.Veiculos.Should().HaveCount(5);
            _context.Servicos.Should().HaveCount(8);
            _context.ItensEstoque.Should().HaveCount(13);
            _context.OrdensServico.Should().HaveCount(3);
        }

        [Fact(DisplayName = "Deve chamar todos os métodos de seed na ordem correta")]
        [Trait("Método", "SeedAll")]
        public void SeedAll_DeveChamarTodosMetodos_NaOrdemCorreta()
        {
            // Act
            SeedData.SeedAll(_context);

            // Assert - Verificar se todos os dados foram criados corretamente
            // Os dados de ordens de serviço só são criados se os outros dados existirem
            // Isso confirma que a ordem está correta
            var ordens = _context.OrdensServico.Include(o => o.ItensIncluidos).Include(o => o.ServicosIncluidos).ToList();
            ordens.Should().HaveCount(3);

            // Verificar se as ordens têm relacionamentos corretos com veículos
            var veiculosIds = _context.Veiculos.Select(v => v.Id).ToList();
            ordens.Should().OnlyContain(o => veiculosIds.Contains(o.VeiculoId));
        }

        #endregion
    }
}
