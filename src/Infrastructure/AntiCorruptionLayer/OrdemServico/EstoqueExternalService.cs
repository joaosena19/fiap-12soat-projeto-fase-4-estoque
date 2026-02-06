using Application.Contracts.Gateways;
using Application.OrdemServico.Dtos.External;
using Application.OrdemServico.Interfaces.External;
using Domain.Estoque.Enums;
using Domain.OrdemServico.Enums;
using Shared.Enums;
using Shared.Exceptions;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar itens de estoque do bounded context de Estoque
    /// </summary>
    public class EstoqueExternalService : IEstoqueExternalService
    {
        private readonly IItemEstoqueGateway _estoqueGateway;

        public EstoqueExternalService(IItemEstoqueGateway estoqueGateway)
        {
            _estoqueGateway = estoqueGateway;
        }

        public async Task<ItemEstoqueExternalDto?> ObterItemEstoquePorIdAsync(Guid itemId)
        {
            var item = await _estoqueGateway.ObterPorIdAsync(itemId);
            
            if (item == null)
                return null;

            return new ItemEstoqueExternalDto
            {
                Id = item.Id,
                Nome = item.Nome.Valor,
                Preco = item.Preco.Valor,
                Quantidade = item.Quantidade.Valor,
                TipoItemIncluido = ConverterTipoItemEstoqueParaTipoItemIncluido(item.TipoItemEstoque.Valor)
            };
        }

        public async Task<bool> VerificarDisponibilidadeAsync(Guid itemId, int quantidadeNecessaria)
        {
            var item = await _estoqueGateway.ObterPorIdAsync(itemId);
            
            if (item == null)
                return false;

            return item.VerificarDisponibilidade(quantidadeNecessaria);
        }

        public async Task AtualizarQuantidadeEstoqueAsync(Guid itemId, int novaQuantidade)
        {
            var item = await _estoqueGateway.ObterPorIdAsync(itemId);
            
            if (item == null)
                throw new DomainException($"Item de estoque com ID {itemId} não encontrado.", ErrorType.ReferenceNotFound);

            item.AtualizarQuantidade(novaQuantidade);
            await _estoqueGateway.AtualizarAsync(item);
        }

        /// <summary>
        /// Converte o tipo de item de estoque (do bounded context de Estoque) 
        /// para o tipo de item incluído (do bounded context de OrdemServico)
        /// </summary>
        private static TipoItemIncluidoEnum ConverterTipoItemEstoqueParaTipoItemIncluido(TipoItemEstoqueEnum tipoItemEstoque)
        {
            return tipoItemEstoque switch
            {
                TipoItemEstoqueEnum.Peca => TipoItemIncluidoEnum.Peca,
                TipoItemEstoqueEnum.Insumo => TipoItemIncluidoEnum.Insumo,
                _ => throw new DomainException($"Tipo de item de estoque '{tipoItemEstoque}' não é válido.", ErrorType.InvalidInput)
            };
        }
    }
}
