using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace API.Presenters.Cadastro.Servico
{
    public class AtualizarServicoPresenter : BasePresenter, IAtualizarServicoPresenter
    {
        public void ApresentarSucesso(ServicoAggregate servico)
        {
            var dto = new RetornoServicoDto
            {
                Id = servico.Id,
                Nome = servico.Nome.Valor,
                Preco = servico.Preco.Valor
            };
            
            DefinirSucesso(dto);
        }
    }
}