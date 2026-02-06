using Application.Identidade.Dtos;
using Application.Contracts.Presenters;

namespace API.Presenters.Identidade.Usuario
{
    public class BuscarUsuarioPorDocumentoPresenter : BasePresenter, IBuscarUsuarioPorDocumentoPresenter
    {
        public void ApresentarSucesso(Domain.Identidade.Aggregates.Usuario usuario)
        {
            var dto = new RetornoUsuarioDto
            {
                Id = usuario.Id,
                DocumentoIdentificador = usuario.DocumentoIdentificadorUsuario.Valor,
                TipoDocumentoIdentificador = usuario.DocumentoIdentificadorUsuario.TipoDocumento.ToString(),
                Roles = usuario.Roles.Select(r => r.Nome.Valor).ToList()
            };
            
            DefinirSucesso(dto);
        }
    }
}