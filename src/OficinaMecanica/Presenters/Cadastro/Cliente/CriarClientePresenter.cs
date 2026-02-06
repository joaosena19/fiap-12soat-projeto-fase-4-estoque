using Application.Cadastros.Dtos;
using Application.Contracts.Presenters;

namespace API.Presenters.Cadastro.Cliente
{
    public class CriarClientePresenter : BasePresenter, ICriarClientePresenter
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
            
            DefinirSucessoComLocalizacao("GetById", "Cliente", new { id = cliente.Id }, dto);
        }
    }
}