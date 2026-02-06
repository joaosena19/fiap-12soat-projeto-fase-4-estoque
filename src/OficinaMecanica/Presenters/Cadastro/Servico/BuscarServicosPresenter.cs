using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace API.Presenters.Cadastro.Servico
{
    public class BuscarServicosPresenter : BasePresenter, IBuscarServicosPresenter
    {
        public void ApresentarSucesso(IEnumerable<ServicoAggregate> servicos)
        {
            var dto = servicos.Select(servico => new RetornoServicoDto
            {
                Id = servico.Id,
                Nome = servico.Nome.Valor,
                Preco = servico.Preco.Valor
            });
            
            DefinirSucesso(dto);
        }
    }
}