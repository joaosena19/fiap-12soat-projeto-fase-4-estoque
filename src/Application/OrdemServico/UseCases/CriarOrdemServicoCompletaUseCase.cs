using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Application.Identidade.Services.Extensions;
using Application.OrdemServico.Dtos;
using Domain.Cadastros.Aggregates;
using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;
using Application.Extensions;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Application.Contracts.Monitoramento;

namespace Application.OrdemServico.UseCases;

/// <summary>
/// Cria uma ordem de serviço completa, incluindo cliente, veículo, serviços e itens de estoque. Tenta buscar clientes e veículos existentes antes de criar novos. Caso serviços ou itens não sejam encontrados, são ignorados.
/// </summary>
public class CriarOrdemServicoCompletaUseCase
{
    public async Task ExecutarAsync(Ator ator, CriarOrdemServicoCompletaDto dto, IOrdemServicoGateway ordemServicoGateway, IClienteGateway clienteGateway, IVeiculoGateway veiculoGateway, IServicoGateway servicoGateway, IItemEstoqueGateway itemEstoqueGateway, ICriarOrdemServicoCompletaPresenter presenter, IAppLogger logger, IMetricsService metricsService)
    {
        try
        {
            if (!ator.PodeGerenciarOrdemServico())
                throw new DomainException("Acesso negado. Apenas administradores podem criar ordens de serviço completas.", ErrorType.NotAllowed, "Acesso negado para criar ordem de serviço completa para usuário {Ator_UsuarioId}", ator.UsuarioId);

            var cliente = await BuscarOuCriarCliente(dto.Cliente, clienteGateway);
            var veiculo = await BuscarOuCriarVeiculo(dto.Veiculo, cliente.Id, veiculoGateway);
            var novaOrdemServico = await CriarOrdemServicoComCodigoUnico(veiculo.Id, ordemServicoGateway);

            await AdicionarServicos(dto.ServicosIds, novaOrdemServico, servicoGateway);
            await AdicionarItens(dto.Itens, novaOrdemServico, itemEstoqueGateway);

            var result = await ordemServicoGateway.SalvarAsync(novaOrdemServico);

            RegistrarMetricaOrdemServicoCriada(result.Id, cliente.Id, ator, metricsService, logger);

            presenter.ApresentarSucesso(result);
        }
        catch (DomainException ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .ComDomainErrorType(ex)
                  .LogInformation(ex.LogTemplate, ex.LogArgs);

            presenter.ApresentarErro(ex.Message, ex.ErrorType);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .LogError(ex, "Erro interno do servidor.");

            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }

    private async Task<Cliente> BuscarOuCriarCliente(ClienteDto clienteDto, IClienteGateway clienteGateway)
    {
        var clienteExistente = await clienteGateway.ObterPorDocumentoAsync(clienteDto.DocumentoIdentificador);
        if (clienteExistente != null) return clienteExistente;

        var novoCliente = Cliente.Criar(clienteDto.Nome, clienteDto.DocumentoIdentificador);
        return await clienteGateway.SalvarAsync(novoCliente);
    }

    private async Task<Veiculo> BuscarOuCriarVeiculo(VeiculoDto veiculoDto, Guid clienteId, IVeiculoGateway veiculoGateway)
    {
        var veiculoExistente = await veiculoGateway.ObterPorPlacaAsync(veiculoDto.Placa);
        if (veiculoExistente != null) return veiculoExistente;

        var novoVeiculo = Veiculo.Criar(
            clienteId,
            veiculoDto.Placa,
            veiculoDto.Modelo,
            veiculoDto.Marca,
            veiculoDto.Cor,
            veiculoDto.Ano,
            veiculoDto.TipoVeiculo);

        return await veiculoGateway.SalvarAsync(novoVeiculo);
    }

    private async Task<OrdemServicoAggregate> CriarOrdemServicoComCodigoUnico(Guid veiculoId, IOrdemServicoGateway ordemServicoGateway)
    {
        OrdemServicoAggregate novaOrdemServico;
        OrdemServicoAggregate? ordemServicoExistente;

        do
        {
            novaOrdemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServicoExistente = await ordemServicoGateway.ObterPorCodigoAsync(novaOrdemServico.Codigo.Valor);
        } while (ordemServicoExistente != null);

        return novaOrdemServico;
    }

    /// <summary>
    /// Adiciona servi�os. Caso n�o encontre o servi�o pelo ID, apenas ignora.
    /// </summary>
    private async Task AdicionarServicos(List<Guid>? servicosIds, OrdemServicoAggregate ordemServico, IServicoGateway servicoGateway)
    {
        if (servicosIds == null || servicosIds.Count == 0) return;

        foreach (var servicoId in servicosIds)
        {
            var servico = await servicoGateway.ObterPorIdAsync(servicoId);
            if (servico == null) continue;
            
            ordemServico.AdicionarServico(servico.Id, servico.Nome.Valor, servico.Preco.Valor);
        }
    }

    /// <summary>
    /// Adiciona itens. Caso n�o encontre o item pelo ID, apenas ignora.
    /// </summary>
    private async Task AdicionarItens(List<ItemDto>? itens, OrdemServicoAggregate ordemServico, IItemEstoqueGateway itemEstoqueGateway)
    {
        if (itens == null || itens.Count == 0) return;

        foreach (var itemDto in itens)
        {
            var itemEstoque = await itemEstoqueGateway.ObterPorIdAsync(itemDto.ItemEstoqueId);
            if (itemEstoque == null) continue;
            
            var tipoItemIncluido = itemEstoque.TipoItemEstoque.Valor == Domain.Estoque.Enums.TipoItemEstoqueEnum.Peca ? TipoItemIncluidoEnum.Peca : TipoItemIncluidoEnum.Insumo;

            ordemServico.AdicionarItem(
                itemEstoque.Id,
                itemEstoque.Nome.Valor,
                itemEstoque.Preco.Valor,
                itemDto.Quantidade,
                tipoItemIncluido);
        }
    }

    /// <summary>
    /// Registra métricas para uma ordem de serviço criada. Não lança exception em caso de falha.
    /// </summary>
    private void RegistrarMetricaOrdemServicoCriada(Guid ordemServicoId, Guid clienteId, Ator ator, IMetricsService metricsService, IAppLogger logger)
    {
        try
        {
            metricsService.RegistrarOrdemServicoCriada(ordemServicoId, clienteId, ator.UsuarioId);
        }
        catch (Exception ex)
        {
            logger.ComUseCase(this)
                  .ComAtor(ator)
                  .LogError(ex, "Erro ao registrar métrica de ordem de serviço criada. OrdemServicoId: {OrdemServicoId}", ordemServicoId);
        }
    }


}