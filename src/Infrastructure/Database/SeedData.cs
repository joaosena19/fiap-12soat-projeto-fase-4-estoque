using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Domain.OrdemServico.Aggregates.OrdemServico;
using Domain.OrdemServico.Enums;
using Domain.Identidade.Aggregates;
using Infrastructure.Authentication.PasswordHashing;
using Microsoft.Extensions.Options;
using Shared.Options;

namespace Infrastructure.Database
{
    public static class SeedData
    {
        public static void SeedClientes(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Clientes.Any())
            {
                return;
            }

            // 2. Cria dados de teste para clientes
            var clientesDeTeste = new List<Cliente>
            {
                Cliente.Criar("João Silva", "56229071010"),
                Cliente.Criar("Maria Santos", "99754534063"),
                Cliente.Criar("Pedro Oliveira", "13763122044"),
                Cliente.Criar("Transportadora Logística Express Ltda", "62255092000108"),
                Cliente.Criar("Auto Peças e Serviços São Paulo S.A.", "13179173000160"),
                Cliente.Criar("cliente", "19649323007") // Cliente com usuário
            };

            // 3. Salva os dados no banco
            context.Clientes.AddRange(clientesDeTeste);
            context.SaveChanges();
        }

        public static void SeedVeiculos(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Veiculos.Any())
                return;

            // Obtém alguns clientes existentes para associar aos veículos
            var clientes = context.Clientes.Take(5).ToList();
            if (clientes.Count < 5)
                return; // Se não há clientes suficientes, não cria veículos

            // 2. Cria dados de teste para veículos
            var veiculosDeTeste = new List<Veiculo>
            {
                Veiculo.Criar(clientes[0].Id, "ABC-1234", "Civic", "Honda", "Prata", 2020, TipoVeiculoEnum.Carro),
                Veiculo.Criar(clientes[1].Id, "XYZ-5678", "Corolla", "Toyota", "Branco", 2019, TipoVeiculoEnum.Carro),
                Veiculo.Criar(clientes[2].Id, "DEF-9012", "CB 600F", "Honda", "Azul", 2021, TipoVeiculoEnum.Moto),
                Veiculo.Criar(clientes[3].Id, "GHI-3456", "Onix", "Chevrolet", "Vermelho", 2022, TipoVeiculoEnum.Carro),
                Veiculo.Criar(clientes[4].Id, "JKL-7890", "YZF-R3", "Yamaha", "Preto", 2020, TipoVeiculoEnum.Moto)
            };

            // 3. Salva os dados no banco
            context.Veiculos.AddRange(veiculosDeTeste);
            context.SaveChanges();
        }

        public static void SeedServicos(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Servicos.Any())
                return;

            // 2. Cria dados de teste para serviços
            var servicosDeTeste = new List<Servico>
            {
                Servico.Criar("Troca de Óleo", 80.00m),
                Servico.Criar("Alinhamento e Balanceamento", 120.00m),
                Servico.Criar("Revisão Completa", 350.00m),
                Servico.Criar("Troca de Pastilhas de Freio", 180.00m),
                Servico.Criar("Troca de Filtro de Ar", 45.00m),
                Servico.Criar("Diagnóstico Eletrônico", 100.00m),
                Servico.Criar("Troca de Correia Dentada", 280.00m),
                Servico.Criar("Limpeza de Bicos Injetores", 150.00m)
            };

            // 3. Salva os dados no banco
            context.Servicos.AddRange(servicosDeTeste);
            context.SaveChanges();
        }

        public static void SeedItensEstoque(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.ItensEstoque.Any())
                return;

            // 2. Cria dados de teste para itens de estoque
            var itensEstoqueDeTeste = new List<ItemEstoque>
            {
                // Peças
                ItemEstoque.Criar("Óleo Motor 5W30", 50, TipoItemEstoqueEnum.Peca, 45.90m),
                ItemEstoque.Criar("Filtro de Óleo", 30, TipoItemEstoqueEnum.Peca, 25.50m),
                ItemEstoque.Criar("Pastilha de Freio Dianteira", 20, TipoItemEstoqueEnum.Peca, 89.90m),
                ItemEstoque.Criar("Pastilha de Freio Traseira", 25, TipoItemEstoqueEnum.Peca, 65.90m),
                ItemEstoque.Criar("Filtro de Ar", 40, TipoItemEstoqueEnum.Peca, 32.90m),
                ItemEstoque.Criar("Correia Dentada", 15, TipoItemEstoqueEnum.Peca, 125.90m),
                ItemEstoque.Criar("Vela de Ignição", 60, TipoItemEstoqueEnum.Peca, 18.90m),
                ItemEstoque.Criar("Disco de Freio", 10, TipoItemEstoqueEnum.Peca, 189.90m),
                
                // Insumos
                ItemEstoque.Criar("Fluido de Freio", 100, TipoItemEstoqueEnum.Insumo, 15.90m),
                ItemEstoque.Criar("Aditivo para Radiador", 80, TipoItemEstoqueEnum.Insumo, 22.50m),
                ItemEstoque.Criar("Graxa Multiuso", 200, TipoItemEstoqueEnum.Insumo, 8.90m),
                ItemEstoque.Criar("Desengraxante", 150, TipoItemEstoqueEnum.Insumo, 12.90m),
                ItemEstoque.Criar("Spray Lubrificante", 120, TipoItemEstoqueEnum.Insumo, 16.50m)
            };

            // 3. Salva os dados no banco
            context.ItensEstoque.AddRange(itensEstoqueDeTeste);
            context.SaveChanges();
        }

        public static void SeedOrdensServico(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.OrdensServico.Any())
                return;

            // Obtém dados necessários para criar as ordens de serviço
            var veiculos = context.Veiculos.Take(3).ToList();
            var servicos = context.Servicos.Take(3).ToList();
            var itensEstoque = context.ItensEstoque.Take(3).ToList();

            if (veiculos.Count < 3 || servicos.Count < 3 || itensEstoque.Count < 3)
                return; // Se não há dados suficientes, não cria ordens de serviço

            var ordensServico = new List<OrdemServico>();

            // 1. Ordem de serviço CANCELADA - apenas com itens incluídos e orçamento
            var osCancelada = OrdemServico.Criar(veiculos[0].Id);
            osCancelada.IniciarDiagnostico(); 
            
            // Adiciona itens
            osCancelada.AdicionarItem(
                itensEstoque[0].Id,
                itensEstoque[0].Nome.Valor,
                itensEstoque[0].Preco.Valor,
                2,
                TipoItemIncluidoEnum.Peca
            );
            osCancelada.AdicionarItem(
                itensEstoque[1].Id,
                itensEstoque[1].Nome.Valor,
                itensEstoque[1].Preco.Valor,
                1,
                TipoItemIncluidoEnum.Insumo
            );
            
            osCancelada.GerarOrcamento();
            osCancelada.DesaprovarOrcamento(); // Cancela a ordem
            
            ordensServico.Add(osCancelada);

            // 2. Ordem de serviço ENTREGUE - apenas com serviços e orçamento
            var osEntregue = OrdemServico.Criar(veiculos[1].Id);
            osEntregue.IniciarDiagnostico();
            
            // Adiciona serviços
            osEntregue.AdicionarServico(servicos[0].Id, servicos[0].Nome.Valor, servicos[0].Preco.Valor);
            osEntregue.AdicionarServico(servicos[1].Id, servicos[1].Nome.Valor, servicos[1].Preco.Valor);
            
            osEntregue.GerarOrcamento();
            osEntregue.AprovarOrcamento();
            osEntregue.FinalizarExecucao(); 
            osEntregue.Entregar();
            
            ordensServico.Add(osEntregue);

            // 3. Ordem de serviço EM DIAGNÓSTICO - sem serviços, itens ou orçamento
            var osEmDiagnostico = OrdemServico.Criar(veiculos[2].Id);
            osEmDiagnostico.IniciarDiagnostico(); // Muda para EmDiagnostico
            
            ordensServico.Add(osEmDiagnostico);

            // 3. Salva os dados no banco
            context.OrdensServico.AddRange(ordensServico);
            context.SaveChanges();
        }

        public static void SeedUsuarios(AppDbContext context)
        {
            // 1. Garante que o banco não será populado novamente
            if (context.Usuarios.Any())
                return;

            // 2. Configura PasswordHasher com opções padrão
            var options = new Argon2HashingOptions
            {
                SaltSize = 16,
                HashSize = 32,
                Iterations = 4,
                MemorySize = 65536,
                DegreeOfParallelism = 1
            };
            var passwordHasher = new PasswordHasher(Options.Create(options));

            // 3. Busca ou cria as roles se elas não existirem (para testes unitários)
            var roleAdmin = context.Roles.FirstOrDefault(r => r.Id == Domain.Identidade.Enums.RoleEnum.Administrador);
            if (roleAdmin == null)
            {
                roleAdmin = Role.Administrador();
                context.Roles.Add(roleAdmin);
            }

            var roleCliente = context.Roles.FirstOrDefault(r => r.Id == Domain.Identidade.Enums.RoleEnum.Cliente);
            if (roleCliente == null)
            {
                roleCliente = Role.Cliente();
                context.Roles.Add(roleCliente);
            }

            context.SaveChanges(); // Salva as roles primeiro se necessário

            // 4. Cria usuários de teste
            var usuariosDeTeste = new List<Usuario>
            {
                // Administrador
                Usuario.Criar("82954150009", passwordHasher.Hash("admin123"), roleAdmin),
                
                // Cliente 
                Usuario.Criar("19649323007", passwordHasher.Hash("cliente123"), roleCliente)
            };

            // 5. Salva os usuários no banco
            context.Usuarios.AddRange(usuariosDeTeste);
            context.SaveChanges();
        }

        public static void SeedAll(AppDbContext context)
        {
            SeedUsuarios(context);
            SeedClientes(context);
            SeedVeiculos(context);
            SeedServicos(context);
            SeedItensEstoque(context);
            SeedOrdensServico(context);
        }
    }
}
