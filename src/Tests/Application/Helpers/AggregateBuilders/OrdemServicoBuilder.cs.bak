using Bogus;
using Domain.OrdemServico.Enums;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.SharedHelpers.AggregateBuilders
{
    public class OrdemServicoBuilder
    {
        private Guid _veiculoId;
        private StatusOrdemServicoEnum? _status;
        private readonly List<ItemData> _itens = new();
        private readonly List<ServicoData> _servicos = new();
        private readonly Faker _faker = new Faker("pt_BR");

        public OrdemServicoBuilder()
        {
            _veiculoId = Guid.NewGuid();
        }

        public OrdemServicoBuilder ComVeiculoId(Guid veiculoId)
        {
            _veiculoId = veiculoId;
            return this;
        }

        public OrdemServicoBuilder ComStatus(StatusOrdemServicoEnum status)
        {
            _status = status;
            return this;
        }

        public OrdemServicoBuilder ComOrcamento()
        {
            // Adiciona itens e serviços se não foram especificados
            if (_itens.Count == 0)
                ComItens();
            if (_servicos.Count == 0)
                ComServicos();
            
            _status = StatusOrdemServicoEnum.AguardandoAprovacao;
            return this;
        }

        public OrdemServicoBuilder ProntoParaOrcamento()
        {
            _status = StatusOrdemServicoEnum.EmDiagnostico;
            return this;
        }

        public OrdemServicoBuilder ProntoParaFinalizacao()
        {
            if (_itens.Count == 0)
                ComItens();
            if (_servicos.Count == 0)
                ComServicos();
            
            _status = StatusOrdemServicoEnum.EmExecucao;
            return this;
        }

        public OrdemServicoBuilder ProntoParaEntrega()
        {
            if (_itens.Count == 0)
                ComItens();
            if (_servicos.Count == 0)
                ComServicos();
            
            _status = StatusOrdemServicoEnum.Finalizada;
            return this;
        }

        public OrdemServicoBuilder Entregue()
        {
            if (_itens.Count == 0)
                ComItens();
            if (_servicos.Count == 0)
                ComServicos();
            
            _status = StatusOrdemServicoEnum.Entregue;
            return this;
        }

        public OrdemServicoBuilder ComItens(params ItemData[] itens)
        {
            if (itens.Length == 0)
            {
                // Gera quantidade fixa de 1 item
                var quantidade = 1;
                for (int i = 0; i < quantidade; i++)
                {
                    _itens.Add(new ItemData(
                        Guid.NewGuid(),
                        _faker.Commerce.ProductName(),
                        _faker.Random.Decimal(10, 500),
                        _faker.Random.Int(1, 5),
                        _faker.PickRandom<TipoItemIncluidoEnum>()
                    ));
                }
            }
            else
            {
                _itens.AddRange(itens);
            }
            return this;
        }

        public OrdemServicoBuilder ComServicos(params ServicoData[] servicos)
        {
            if (servicos.Length == 0)
            {
                // Gera quantidade fixa de 1 serviço
                var quantidade = 1;
                for (int i = 0; i < quantidade; i++)
                {
                    _servicos.Add(new ServicoData(
                        Guid.NewGuid(),
                        _faker.Commerce.ProductName(),
                        _faker.Random.Decimal(30, 200)
                    ));
                }
            }
            else
            {
                _servicos.AddRange(servicos);
            }
            return this;
        }

        public OrdemServicoAggregate Build()
        {
            var ordemServico = OrdemServicoAggregate.Criar(_veiculoId);
            
            // Adiciona itens se especificados
            foreach (var item in _itens)
            {
                ordemServico.AdicionarItem(item.Id, item.Nome, item.Preco, item.Quantidade, item.Tipo);
            }
            
            // Adiciona serviços se especificados
            foreach (var servico in _servicos)
            {
                ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Preco);
            }
            
            // Aplica o status desejado
            if (_status.HasValue)
            {
                switch (_status.Value)
                {
                    case StatusOrdemServicoEnum.EmDiagnostico:
                        ordemServico.IniciarDiagnostico();
                        break;
                    case StatusOrdemServicoEnum.AguardandoAprovacao:
                        ordemServico.IniciarDiagnostico();
                        // Garante que existe pelo menos um item ou serviço para permitir o orçamento
                        if (!ordemServico.ItensIncluidos.Any() && !ordemServico.ServicosIncluidos.Any())
                        {
                            ordemServico.AdicionarServico(Guid.NewGuid(), _faker.Commerce.ProductName(), _faker.Random.Decimal(30, 200));
                        }
                        ordemServico.GerarOrcamento();
                        break;
                    case StatusOrdemServicoEnum.EmExecucao:
                        ordemServico.IniciarDiagnostico();
                        // Garante que existe pelo menos um item ou serviço para permitir o orçamento e aprovação
                        if (!ordemServico.ItensIncluidos.Any() && !ordemServico.ServicosIncluidos.Any())
                        {
                            ordemServico.AdicionarServico(Guid.NewGuid(), _faker.Commerce.ProductName(), _faker.Random.Decimal(30, 200));
                        }
                        ordemServico.GerarOrcamento();
                        ordemServico.AprovarOrcamento();
                        break;
                    case StatusOrdemServicoEnum.Finalizada:
                        ordemServico.IniciarDiagnostico();
                        // Garante que existe pelo menos um item ou serviço para permitir todo o fluxo
                        if (!ordemServico.ItensIncluidos.Any() && !ordemServico.ServicosIncluidos.Any())
                        {
                            ordemServico.AdicionarServico(Guid.NewGuid(), _faker.Commerce.ProductName(), _faker.Random.Decimal(30, 200));
                        }
                        ordemServico.GerarOrcamento();
                        ordemServico.AprovarOrcamento();
                        ordemServico.FinalizarExecucao();
                        break;
                    case StatusOrdemServicoEnum.Entregue:
                        ordemServico.IniciarDiagnostico();
                        // Garante que existe pelo menos um item ou serviço para permitir todo o fluxo
                        if (!ordemServico.ItensIncluidos.Any() && !ordemServico.ServicosIncluidos.Any())
                        {
                            ordemServico.AdicionarServico(Guid.NewGuid(), _faker.Commerce.ProductName(), _faker.Random.Decimal(30, 200));
                        }
                        ordemServico.GerarOrcamento();
                        ordemServico.AprovarOrcamento();
                        ordemServico.FinalizarExecucao();
                        ordemServico.Entregar();
                        break;
                }
            }
            
            return ordemServico;
        }
    }

    public record ItemData(Guid Id, string Nome, decimal Preco, int Quantidade, TipoItemIncluidoEnum Tipo);
    public record ServicoData(Guid Id, string Nome, decimal Preco);
}