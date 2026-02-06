using Application.Estoque.UseCases;
using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.Identidade.Services;
using Domain.Estoque.Enums;
using Infrastructure.Handlers;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Handlers.Estoque
{
    public class ItemEstoqueHandler : BaseHandler
    {
        public ItemEstoqueHandler(ILoggerFactory loggerFactory) : base(loggerFactory) { }

        public async Task CriarItemEstoqueAsync(Ator ator, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, ICriarItemEstoquePresenter presenter)
        {
            var useCase = new CriarItemEstoqueUseCase();
            var logger = CriarLoggerPara<CriarItemEstoqueUseCase>();
            
            await useCase.ExecutarAsync(ator, nome, quantidade, tipoItemEstoque, preco, gateway, presenter, logger);
        }

        public async Task AtualizarItemEstoqueAsync(Ator ator, Guid id, string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque, decimal preco, IItemEstoqueGateway gateway, IAtualizarItemEstoquePresenter presenter)
        {
            var useCase = new AtualizarItemEstoqueUseCase();
            var logger = CriarLoggerPara<AtualizarItemEstoqueUseCase>();

            await useCase.ExecutarAsync(ator, id, nome, quantidade, tipoItemEstoque, preco, gateway, presenter, logger);
        }

        public async Task AtualizarQuantidadeAsync(Ator ator, Guid id, int quantidade, IItemEstoqueGateway gateway, IAtualizarQuantidadePresenter presenter)
        {
            var useCase = new AtualizarQuantidadeUseCase();
            var logger = CriarLoggerPara<AtualizarQuantidadeUseCase>();

            await useCase.ExecutarAsync(ator, id, quantidade, gateway, presenter, logger);
        }

        public async Task BuscarTodosItensEstoqueAsync(Ator ator, IItemEstoqueGateway gateway, IBuscarTodosItensEstoquePresenter presenter)
        {
            var useCase = new BuscarTodosItensEstoqueUseCase();
            var logger = CriarLoggerPara<BuscarTodosItensEstoqueUseCase>();
            
            await useCase.ExecutarAsync(ator, gateway, presenter, logger);
        }

        public async Task BuscarItemEstoquePorIdAsync(Ator ator, Guid id, IItemEstoqueGateway gateway, IBuscarItemEstoquePorIdPresenter presenter)
        {
            var useCase = new BuscarItemEstoquePorIdUseCase();
            var logger = CriarLoggerPara<BuscarItemEstoquePorIdUseCase>();

            await useCase.ExecutarAsync(ator, id, gateway, presenter, logger);
        }

        public async Task VerificarDisponibilidadeAsync(Ator ator, Guid id, int quantidadeRequisitada, IItemEstoqueGateway gateway, IVerificarDisponibilidadePresenter presenter)
        {
            var useCase = new VerificarDisponibilidadeUseCase();
            var logger = CriarLoggerPara<VerificarDisponibilidadeUseCase>();
            
            await useCase.ExecutarAsync(ator, id, quantidadeRequisitada, gateway, presenter, logger);
        }
    }
}