using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;

namespace API.Presenters.Cadastro.Veiculo
{
    public class BuscarVeiculoPorIdPresenter : BasePresenter, IBuscarVeiculoPorIdPresenter
    {
        public void ApresentarSucesso(Domain.Cadastros.Aggregates.Veiculo veiculo)
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