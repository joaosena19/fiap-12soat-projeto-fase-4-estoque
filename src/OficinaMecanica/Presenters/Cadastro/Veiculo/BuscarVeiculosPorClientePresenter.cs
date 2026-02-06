using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;

namespace API.Presenters.Cadastro.Veiculo
{
    public class BuscarVeiculosPorClientePresenter : BasePresenter, IBuscarVeiculosPorClientePresenter
    {
        public void ApresentarSucesso(IEnumerable<Domain.Cadastros.Aggregates.Veiculo> veiculos)
        {
            var dto = veiculos.Select(veiculo => new RetornoVeiculoDto
            {
                Id = veiculo.Id,
                ClienteId = veiculo.ClienteId,
                Placa = veiculo.Placa.Valor,
                Modelo = veiculo.Modelo.Valor,
                Marca = veiculo.Marca.Valor,
                Cor = veiculo.Cor.Valor,
                Ano = veiculo.Ano.Valor,
                TipoVeiculo = veiculo.TipoVeiculo.Valor.ToString()
            });
            
            DefinirSucesso(dto);
        }
    }
}