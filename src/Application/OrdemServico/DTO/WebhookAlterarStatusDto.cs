using Domain.OrdemServico.Enums;

namespace Application.OrdemServico.Dtos
{
    public class WebhookAlterarStatusDto
    {
        public Guid Id { get; set; }
        public StatusOrdemServicoEnum Status { get; set; }
    }
}
