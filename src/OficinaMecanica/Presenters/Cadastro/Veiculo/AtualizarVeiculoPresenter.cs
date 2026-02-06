using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace API.Presenters.Cadastro.Veiculo
{
    public class AtualizarVeiculoPresenter : BasePresenter, IAtualizarVeiculoPresenter
    {
        public void ApresentarSucesso(VeiculoAggregate veiculo)
        {
            var dto = new RetornoVeiculoDto
            {
                Id = veiculo.Id,
                ClienteId = veiculo.ClienteId,
                Placa = veiculo.Placa.Valor,
                Modelo = veiculo.Modelo.Valor,
                Marca = veiculo.Marca.Valor,
                Cor = veiculo.Cor.Valor,
                Ano = veiculo.Ano.Valor,
                TipoVeiculo = veiculo.TipoVeiculo.Valor.ToString()
            };
            
            DefinirSucesso(dto);
        }
    }
}