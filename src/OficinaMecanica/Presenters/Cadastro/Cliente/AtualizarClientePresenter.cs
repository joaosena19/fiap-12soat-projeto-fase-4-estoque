using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;

namespace API.Presenters.Cadastro.Cliente
{
    public class AtualizarClientePresenter : BasePresenter, IAtualizarClientePresenter
    {
        public void ApresentarSucesso(Domain.Cadastros.Aggregates.Cliente cliente)
        {
            var dto = new RetornoClienteDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome.Valor,
                DocumentoIdentificador = cliente.DocumentoIdentificador.Valor,
                TipoDocumentoIdentificador = cliente.DocumentoIdentificador.TipoDocumento.ToString()
            };
            
            DefinirSucesso(dto);
        }
    }
}