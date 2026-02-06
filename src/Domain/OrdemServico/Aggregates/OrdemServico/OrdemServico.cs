using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.OrdemServico;
using Shared.Exceptions;
using Shared.Enums;
using UUIDNext;
using Shared.Attributes;

namespace Domain.OrdemServico.Aggregates.OrdemServico
{
    [AggregateRoot]
    public class OrdemServico
    {
        public Guid Id { get; private init; }
        public Guid VeiculoId { get; private set; }
        public Codigo Codigo { get; private set; } = null!;
        public Status Status { get; private set; } = null!;
        public HistoricoTemporal Historico { get; private set; }


        private readonly List<ServicoIncluido> _servicosIncluidos = new();
        private readonly List<ItemIncluido> _itensIncluidos = new();
        public IReadOnlyCollection<ServicoIncluido> ServicosIncluidos => _servicosIncluidos.AsReadOnly();
        public IReadOnlyCollection<ItemIncluido> ItensIncluidos => _itensIncluidos.AsReadOnly();

        public Orcamento? Orcamento { get; private set; } = null;

        // Construtor sem parâmetro para EF Core
        private OrdemServico() 
        {
            Historico = new HistoricoTemporal(DateTime.UtcNow);
        }

        private OrdemServico(Guid id, Guid veiculoId, Codigo codigo, Status status, HistoricoTemporal historico)
        {
            Id = id;
            VeiculoId = veiculoId;
            Codigo = codigo;
            Status = status;
            Historico = historico;
        }

        public static OrdemServico Criar(Guid veiculoId)
        {
            return new OrdemServico(
                Uuid.NewSequential(),
                veiculoId,
                Codigo.GerarNovo(),
                new Status(StatusOrdemServicoEnum.Recebida),
                new HistoricoTemporal(DateTime.UtcNow)
            );
        }

        public bool PermiteAlterarServicosItens()
        {
            var statusPermitidos = new List<StatusOrdemServicoEnum>() { StatusOrdemServicoEnum.Recebida, StatusOrdemServicoEnum.EmDiagnostico };
            return statusPermitidos.Contains(Status.Valor);            
        }

        public void AdicionarServico(Guid servicoOriginalId, string nome, decimal preco)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível adicionar serviços a uma Ordem de Serviço com o status '{Status.Valor}'.", ErrorType.DomainRuleBroken);

            if (_servicosIncluidos.Any(s => s.ServicoOriginalId == servicoOriginalId))
                throw new DomainException("Este serviço já foi incluído nesta OS.", ErrorType.DomainRuleBroken);

            var novoServico = ServicoIncluido.Criar(servicoOriginalId, nome, preco);
            _servicosIncluidos.Add(novoServico);
        }

        public void AdicionarItem(Guid itemEstoqueOriginalId, string nome, decimal precoUnitario, int quantidade, TipoItemIncluidoEnum tipo)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível adicionar itens a uma Ordem de Serviço com o status '{Status.Valor}'.", ErrorType.DomainRuleBroken);

            if (quantidade <= 0)
                throw new DomainException("A quantidade deve ser maior que zero.", ErrorType.InvalidInput);

            var itemExistente = _itensIncluidos.FirstOrDefault(i => i.ItemEstoqueOriginalId == itemEstoqueOriginalId);

            if (itemExistente is not null)
            {
                itemExistente.IncrementarQuantidade(quantidade);
            }
            else
            {
                var novoItem = ItemIncluido.Criar(itemEstoqueOriginalId, nome, precoUnitario, quantidade, tipo);
                _itensIncluidos.Add(novoItem);
            }
        }

        public void RemoverServico(Guid servicoIncluidoId)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível remover serviços de uma Ordem de Serviço com o status '{Status.Valor}'.", ErrorType.DomainRuleBroken);

            var servicoParaRemover = _servicosIncluidos.FirstOrDefault(s => s.Id == servicoIncluidoId);
            if (servicoParaRemover == null)
                throw new DomainException("Serviço não encontrado nesta ordem de serviço.", ErrorType.ResourceNotFound);

            _servicosIncluidos.Remove(servicoParaRemover);
        }

        public void RemoverItem(Guid itemIncluidoId)
        {
            if (!PermiteAlterarServicosItens())
                throw new DomainException($"Não é possível remover itens de uma Ordem de Serviço com o status '{Status.Valor}'.", ErrorType.DomainRuleBroken);

            var itemParaRemover = _itensIncluidos.FirstOrDefault(i => i.Id == itemIncluidoId);
            if (itemParaRemover == null)
                throw new DomainException("Item não encontrado nesta ordem de serviço.", ErrorType.ResourceNotFound);

            _itensIncluidos.Remove(itemParaRemover);
        }

        public void Cancelar()
        {
            Status = new Status(StatusOrdemServicoEnum.Cancelada);
        }

        public void IniciarDiagnostico()
        {
            if (Status.Valor != StatusOrdemServicoEnum.Recebida)
                throw new DomainException($"Só é possível iniciar diagnóstico para um ordem de serviço com o status '{StatusOrdemServicoEnum.Recebida}'", ErrorType.DomainRuleBroken);

            Status = new Status(StatusOrdemServicoEnum.EmDiagnostico);
        }
        
        public void GerarOrcamento()
        {
            if (Orcamento != null)
                throw new DomainException("Já existe um orçamento gerado para esta ordem de serviço.", ErrorType.Conflict);

            if (Status.Valor != StatusOrdemServicoEnum.EmDiagnostico)
                throw new DomainException($"Só é possível gerar orçamento para uma ordem de serviço com o status '{StatusOrdemServicoEnum.EmDiagnostico}'", ErrorType.DomainRuleBroken);

            if (!ServicosIncluidos.Any() && !ItensIncluidos.Any())
                throw new DomainException("Não é possível gerar orçamento sem pelo menos um serviço ou item incluído.", ErrorType.DomainRuleBroken);

            Status = new Status(StatusOrdemServicoEnum.AguardandoAprovacao);
            Orcamento = Orcamento.GerarOrcamento(ServicosIncluidos, ItensIncluidos);
        }

        public void AprovarOrcamento()
        {
            if (Orcamento == null)
                throw new DomainException("Não existe orçamento para aprovar. É necessário gerar o orçamento primeiro.", ErrorType.DomainRuleBroken);

            IniciarExecucao();
        }

        public void DesaprovarOrcamento()
        {
            if (Orcamento == null)
                throw new DomainException("Não existe orçamento para desaprovar. É necessário gerar o orçamento primeiro.", ErrorType.DomainRuleBroken);

            Cancelar();
        }

        public void IniciarExecucao()
        {
            if (Status.Valor != StatusOrdemServicoEnum.AguardandoAprovacao)
                throw new DomainException($"Só é possível iniciar execução para uma ordem de serviço com o status '{StatusOrdemServicoEnum.AguardandoAprovacao}'", ErrorType.DomainRuleBroken);

            Status = new Status(StatusOrdemServicoEnum.EmExecucao);
            Historico = Historico.MarcarDataInicioExecucao();
        }

        public void FinalizarExecucao()
        {
            if (Status.Valor != StatusOrdemServicoEnum.EmExecucao)
                throw new DomainException($"Só é possível finalizar execução para uma ordem de serviço com o status '{StatusOrdemServicoEnum.EmExecucao}'", ErrorType.DomainRuleBroken);

            Status = new Status(StatusOrdemServicoEnum.Finalizada);
            Historico = Historico.MarcarDataFinalizadaExecucao();
        }

        public void Entregar()
        {
            if (Status.Valor != StatusOrdemServicoEnum.Finalizada)
                throw new DomainException($"Só é possível entregar uma ordem de serviço com o status '{StatusOrdemServicoEnum.Finalizada}'", ErrorType.DomainRuleBroken);

            Status = new Status(StatusOrdemServicoEnum.Entregue);
            Historico = Historico.MarcarDataEntrega();
        }

        public void AlterarStatus(StatusOrdemServicoEnum novoStatus)
        {
            switch (novoStatus)
            {
                case StatusOrdemServicoEnum.Cancelada:
                    Cancelar();
                    break;
                case StatusOrdemServicoEnum.EmDiagnostico:
                    IniciarDiagnostico();
                    break;
                case StatusOrdemServicoEnum.AguardandoAprovacao:
                    GerarOrcamento();
                    break;
                case StatusOrdemServicoEnum.EmExecucao:
                    IniciarExecucao();
                    break;
                case StatusOrdemServicoEnum.Finalizada:
                    FinalizarExecucao();
                    break;
                case StatusOrdemServicoEnum.Entregue:
                    Entregar();
                    break;
                case StatusOrdemServicoEnum.Recebida:
                    throw new DomainException($"Não é possível alterar o status para '{StatusOrdemServicoEnum.Recebida}'.", ErrorType.DomainRuleBroken);
                default:
                    throw new DomainException("Status inválido.", ErrorType.InvalidInput);
            }
        }
    }
}
    