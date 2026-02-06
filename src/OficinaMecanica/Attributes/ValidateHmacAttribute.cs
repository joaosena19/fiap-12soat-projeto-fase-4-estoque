using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Infrastructure.Authentication;
using System.Text;

namespace API.Attributes
{
    /// <summary>
    /// Atributo para validação de assinatura HMAC em webhooks
    /// </summary>
    public class ValidateHmacAttribute : Attribute, IAsyncResourceFilter
    {
        private const string SIGNATURE_HEADER = "X-Signature";

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var request = context.HttpContext.Request;
            
            // Obter a assinatura do header
            if (!request.Headers.TryGetValue(SIGNATURE_HEADER, out var signatureValues))
            {
                context.Result = new UnauthorizedObjectResult(new { error = "HMAC signature ausente" });
                return;
            }

            var signature = signatureValues.FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
            {
                context.Result = new UnauthorizedObjectResult(new { error = "HMAC signature em formato inválido" });
                return;
            }

            // Ler o body da requisição
            request.EnableBuffering();
            request.Body.Position = 0;
            string payload;
            using (var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true))
            {
                payload = await reader.ReadToEndAsync();
            }
            request.Body.Position = 0; // Reset para permitir model binding e leitura no controller

            // Validar a assinatura
            var hmacService = context.HttpContext.RequestServices.GetRequiredService<IHmacValidationService>();
            
            if (!hmacService.ValidateSignature(payload, signature))
            {
                context.Result = new UnauthorizedObjectResult(new { error = "HMAC signature inválida" });
                return;
            }

            await next();
        }
    }
}