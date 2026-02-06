using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;

namespace API.Presenters.Cadastro.Cliente
{
    public class BuscarClientesPresenter : BasePresenter, IBuscarClientesPresenter
    {
        public void ApresentarSucesso(IEnumerable<Domain.Cadastros.Aggregates.Cliente> clientes)
        {
            var dto = clientes.Select(cliente => new RetornoClienteDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome.Valor,
                DocumentoIdentificador = cliente.DocumentoIdentificador.Valor,
                TipoDocumentoIdentificador = cliente.DocumentoIdentificador.TipoDocumento.ToString()
            });
            
            DefinirSucesso(dto);
        }
    }
}